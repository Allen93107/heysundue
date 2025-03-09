using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Heysundue.Models;

namespace Heysundue.Controllers
{
    public class ExcelController : Controller
    {
        private readonly ArticleContext _context;

        public ExcelController(ArticleContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile fileUpload, int? meetingId, string target)
        {
            if (fileUpload == null || fileUpload.Length <= 0)
            {
                TempData["ErrorMessage"] = "Please upload a valid Excel file.";
                return RedirectToAction("Upload");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await fileUpload.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed();
                        var headerRow = rows.First();
                        var columnMapping = GetColumnMapping(headerRow);

                        foreach (var row in rows.Skip(1))
                        {
                            var idNumber = row.Cell(columnMapping["idnumber"]).GetValue<string>();
                            var firstName = row.Cell(columnMapping["firstname"]).GetValue<string>();
                            var lastName = row.Cell(columnMapping["lastname"]).GetValue<string>();

                            if (target == "joinlist")
                            {
                                await ProcessJoinlistAsync(row, idNumber, firstName, lastName, columnMapping);
                            }
                            else if (target == "sessionuser" && meetingId.HasValue)
                            {
                                await ProcessSessionuserAsync(row, idNumber, firstName, lastName, meetingId.Value, columnMapping);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Joinlist2", "Joinlist");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                return RedirectToAction("Upload");
            }
        }

        private async Task ProcessJoinlistAsync(IXLRangeRow row, string idNumber, string firstName, string lastName, Dictionary<string, int> columnMapping)
        {
            Joinlist existingJoinlist = null;

            if (!string.IsNullOrEmpty(idNumber))
            {
                existingJoinlist = await _context.Joinlists.FirstOrDefaultAsync(j => j.IDNumber == idNumber);
            }
            else
            {
                existingJoinlist = await _context.Joinlists.FirstOrDefaultAsync(j => j.FirstName == firstName && j.LastName == lastName);
            }

            if (existingJoinlist != null)
            {
                UpdateJoinlist(existingJoinlist, row, columnMapping);
                _context.Joinlists.Update(existingJoinlist);
            }
            else
            {
                var newJoinlist = CreateJoinlist(row, columnMapping);
                await _context.Joinlists.AddAsync(newJoinlist);
            }
        }

        private async Task ProcessSessionuserAsync(IXLRangeRow row, string idNumber, string firstName, string lastName, int meetingId, Dictionary<string, int> columnMapping)
        {
            var sessionuser = await _context.Sessionusers
                .FirstOrDefaultAsync(s => s.IDNumber == idNumber && s.MeetingID == meetingId);

            if (sessionuser == null)
            {
                var joinlist = await _context.Joinlists
                    .FirstOrDefaultAsync(j => j.IDNumber == idNumber || (j.FirstName == firstName && j.LastName == lastName));

                if (joinlist != null)
                {
                    // 更新 Joinlist 資料
                    UpdateJoinlist(joinlist, row, columnMapping);
                    _context.Joinlists.Update(joinlist);
                }
                else
                {
                    // 新增 Joinlist 資料
                    var newJoinlist = CreateJoinlist(row, columnMapping);
                    await _context.Joinlists.AddAsync(newJoinlist);
                }

                // 新增 Sessionuser 資料
                var newSessionuser = CreateSessionuser(row, meetingId, columnMapping);
                await _context.Sessionusers.AddAsync(newSessionuser);
            }
        }

        private void UpdateJoinlist(Joinlist joinlist, IXLRangeRow row, Dictionary<string, int> columnMapping)
        {
            joinlist.FirstName = row.Cell(columnMapping["firstname"]).GetValue<string>();
            joinlist.LastName = row.Cell(columnMapping["lastname"]).GetValue<string>();
            joinlist.ChineseName = row.Cell(columnMapping["chinesename"]).GetValue<string>();
            joinlist.Country = row.Cell(columnMapping["country"]).GetValue<string>();
            joinlist.RegNo = row.Cell(columnMapping["regno"]).GetValue<string>();
            joinlist.Email = row.Cell(columnMapping["email"]).GetValue<string>();
            joinlist.City = row.Cell(columnMapping["city"]).GetValue<string>();
            joinlist.IdentityType1 = row.Cell(columnMapping["identitytype1"]).GetValue<string>();
            joinlist.IdentityType2 = row.Cell(columnMapping["identitytype2"]).GetValue<string>();
            joinlist.Remark = row.Cell(columnMapping["remark"]).GetValue<string>();
            joinlist.Phone = row.Cell(columnMapping["phone"]).GetValue<string>();
            joinlist.Speaker = row.Cell(columnMapping["speaker"]).GetValue<string>();
        }

        private Joinlist CreateJoinlist(IXLRangeRow row, Dictionary<string, int> columnMapping)
        {
            return new Joinlist
            {
                IDNumber = row.Cell(columnMapping["idnumber"]).GetValue<string>(),
                FirstName = row.Cell(columnMapping["firstname"]).GetValue<string>(),
                LastName = row.Cell(columnMapping["lastname"]).GetValue<string>(),
                ChineseName = row.Cell(columnMapping["chinesename"]).GetValue<string>(),
                Country = row.Cell(columnMapping["country"]).GetValue<string>(),
                RegNo = row.Cell(columnMapping["regno"]).GetValue<string>(),
                Email = row.Cell(columnMapping["email"]).GetValue<string>(),
                City = row.Cell(columnMapping["city"]).GetValue<string>(),
                IdentityType1 = row.Cell(columnMapping["identitytype1"]).GetValue<string>(),
                IdentityType2 = row.Cell(columnMapping["identitytype2"]).GetValue<string>(),
                Remark = row.Cell(columnMapping["remark"]).GetValue<string>(),
                Phone = row.Cell(columnMapping["phone"]).GetValue<string>(),
                Speaker = row.Cell(columnMapping["speaker"]).GetValue<string>()
            };
        }

        private Sessionuser CreateSessionuser(IXLRangeRow row, int meetingId, Dictionary<string, int> columnMapping)
        {
            return new Sessionuser
            {
                MeetingID = meetingId,
                IDNumber = row.Cell(columnMapping["idnumber"]).GetValue<string>(),
                FirstName = row.Cell(columnMapping["firstname"]).GetValue<string>(),
                LastName = row.Cell(columnMapping["lastname"]).GetValue<string>(),
                ChineseName = row.Cell(columnMapping["chinesename"]).GetValue<string>(),
                Country = row.Cell(columnMapping["country"]).GetValue<string>(),
                RegNo = row.Cell(columnMapping["regno"]).GetValue<string>(),
                Email = row.Cell(columnMapping["email"]).GetValue<string>(),
                City = row.Cell(columnMapping["city"]).GetValue<string>(),
                IdentityType1 = row.Cell(columnMapping["identitytype1"]).GetValue<string>(),
                IdentityType2 = row.Cell(columnMapping["identitytype2"]).GetValue<string>(),
                Remark = row.Cell(columnMapping["remark"]).GetValue<string>(),
                Phone = row.Cell(columnMapping["phone"]).GetValue<string>(),
                Speaker = row.Cell(columnMapping["speaker"]).GetValue<string>()
            };
        }

        private Dictionary<string, int> GetColumnMapping(IXLRangeRow headerRow)
        {
            var columnMapping = new Dictionary<string, int>();
            for (int i = 1; i <= headerRow.CellCount(); i++)
            {
                var columnName = headerRow.Cell(i).GetValue<string>().Trim().ToLower().Replace(" ", "");
                columnMapping[columnName] = i;
            }
            return columnMapping;
        }
        [HttpGet]
public IActionResult Upload()
{
    return View(); // 預設會尋找 Views/Excel/Upload.cshtml
}

    }
}
