using Microsoft.AspNetCore.Mvc;
using Heysundue.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Packaging;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using System.Drawing;
using System.Drawing.Imaging;

// 使用別名來區分 Word 和 Drawing 命名空間
using W = DocumentFormat.OpenXml.Wordprocessing;
using D = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;


namespace Heysundue.Controllers
{
    public class SessionuserController : Controller
    {
        private readonly ArticleContext _context;
        private readonly ILogger<SessionuserController> _logger;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;

        public IActionResult GetMeetings()
        {
            var meetings = _context.Meetings.ToList();
            return Json(meetings);
        }

        public IActionResult Sessionregister()
        {
            return View("Sessionregister");
        }


        public SessionuserController(ArticleContext context, ILogger<SessionuserController> logger, ICompositeViewEngine viewEngine, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
        }



        public IActionResult GenerateIdCard(int id)
        {
            var user = _context.Sessionusers.FirstOrDefault(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new W.Document(); // 修正 Document
                    W.Body body = new W.Body(); // 修正 Body
                    mainPart.Document.Append(body);

                    // 上方顯示 RegNo
                    AddCenteredStyledParagraph(body, user.RegNo, 24);

                    // 中間放大顯示 ChineseName
                    AddCenteredStyledParagraph(body, user.ChineseName, 48, isBold: true);

                    // 下方顯示 Country
                    AddCenteredStyledParagraph(body, user.Country, 36);

                    mainPart.Document.Save();
                }

                ms.Seek(0, SeekOrigin.Begin);
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "IDCard.docx");
            }
        }

    
public IActionResult GenerateCertificateCard(int id)
        {
            var user = _context.Sessionusers.FirstOrDefault(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new W.Document();
                    W.Body body = new W.Body(); // 修正 Body
                    mainPart.Document.Append(body);

                    // 顯示 Full Name
                    string fullName = $"{user.FirstName} {user.LastName}";
                    AddCenteredStyledParagraph(body, fullName, 24, isBold: true);

                    // 顯示 Chinese Name
                    AddCenteredStyledParagraph(body, user.ChineseName, 48, isBold: true);

                    // 生成條碼圖片
                    byte[] barcodeImage = GenerateBarcodeImage(user.IDNumber);
                    AddImageToDocument(mainPart, barcodeImage);

                    mainPart.Document.Save();
                }

                ms.Seek(0, SeekOrigin.Begin);
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "CertificateCard.docx");
            }
        }



    // Helper method for adding centered styled paragraph
 private static void AddCenteredStyledParagraph(W.Body body, string textContent, int fontSize, bool isBold = false)
        {
            var runProperties = new W.RunProperties(
                new W.RunFonts { Ascii = "Arial" },
                new W.FontSize { Val = (fontSize * 2).ToString() } 
            );

            if (isBold)
            {
                runProperties.Bold = new W.Bold();
            }

            var run = new W.Run();
            run.Append(runProperties);
            run.Append(new W.Text(textContent ?? ""));

            var paragraph = new W.Paragraph();
            paragraph.Append(run);

            paragraph.ParagraphProperties = new W.ParagraphProperties
            {
                Justification = new W.Justification { Val = W.JustificationValues.Center }
            };

            body.Append(paragraph);
        }

private byte[] GenerateBarcodeImage(string barcodeValue)
{
    var writer = new BarcodeWriter
    {
        Format = BarcodeFormat.CODE_128,
        Options = new EncodingOptions
        {
            Height = 150,  // 調高條碼高度
            Width = 500,   // 調寬條碼寬度
            Margin = 10
        }
    };

    using (Bitmap bitmap = writer.Write(barcodeValue))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}


private void AddImageToDocument(MainDocumentPart mainPart, byte[] imageBytes)
{
    ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);
    using (MemoryStream ms = new MemoryStream(imageBytes))
    {
        imagePart.FeedData(ms);
    }

    string relationshipId = mainPart.GetIdOfPart(imagePart);

    // ✅ 設定圖片尺寸 - 調整條碼變大
    long barcodeWidth = 3000000L;  // 30 cm (調整條碼寬度)
    long barcodeHeight = 1200000L; // 12 cm (調整條碼高度)

    var inline = new DW.Inline(
        new DW.Extent { Cx = barcodeWidth, Cy = barcodeHeight },
        new DW.DocProperties { Id = 1U, Name = "Barcode Image" },
        new DW.NonVisualGraphicFrameDrawingProperties(new D.GraphicFrameLocks { NoChangeAspect = true }),
        new D.Graphic(
            new D.GraphicData(
                new D.Pictures.Picture(
                    new D.Pictures.NonVisualPictureProperties(
                        new D.Pictures.NonVisualDrawingProperties { Id = 0U, Name = "Barcode.png" },
                        new D.Pictures.NonVisualPictureDrawingProperties()),
                    new D.Pictures.BlipFill(
                        new D.Blip { Embed = relationshipId },
                        new D.Stretch(new D.FillRectangle())),
                    new D.Pictures.ShapeProperties(
                        new D.Transform2D(
                            new D.Offset { X = 0L, Y = 0L },
                            new D.Extents { Cx = barcodeWidth, Cy = barcodeHeight }),
                        new D.PresetGeometry(new D.AdjustValueList()) { Preset = D.ShapeTypeValues.Rectangle }))
            ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
    );

    // ✅ 使用 `W.Justification` 讓條碼置中
    W.Paragraph paragraph = new W.Paragraph(
        new W.ParagraphProperties(
            new W.Justification { Val = W.JustificationValues.Center } // ✅ 讓條碼置中
        ),
        new W.Run(inline)
    );

    mainPart.Document.Body.Append(paragraph);
}








[HttpGet]
public JsonResult LoadSessionUsers(int meetingId)
{
    var users = _context.Sessionusers
                .Where(u => u.MeetingID == meetingId)
                .Select(u => new {
                    u.RegNo,
                    u.FirstName,
                    u.LastName,
                    u.ChineseName,
                    u.Country,
                    RegistrationStatus = u.RegistrationStatus,  // 直接回傳不做處理
                    u.Barcode,
                    u.Email,
                    u.City,
                    u.IdentityType1,
                    u.IdentityType2,
                    u.IDNumber,
                    u.ID
                })
                .ToList();

    return Json(users);
}

// 新增會議的方法
[HttpPost]
public IActionResult AddMeeting(string title, string place, DateTime meetingDateTime)
{
    if (string.IsNullOrEmpty(title))
    {
        return Json(new { success = false, message = "會議名稱不能為空" });
    }

    if (string.IsNullOrEmpty(place))
    {
        return Json(new { success = false, message = "地點不能為空" });
    }

    if (meetingDateTime == default)
    {
        return Json(new { success = false, message = "請輸入有效的會議時間" });
    }

    // ✅ 檢查是否已經有相同名稱的會議
    bool meetingExists = _context.Meetings.Any(m => m.Title == title);
    if (meetingExists)
    {
        return Json(new { success = false, message = "會議名稱已存在，請使用不同名稱" });
    }

    var meeting = new Meeting
    {
        Title = title,
        Place = place,
        CreatedDate = DateTime.Now,
        MeetingDateTime = meetingDateTime
    };

    _context.Meetings.Add(meeting);
    _context.SaveChanges();

    return Json(new { success = true, meetingId = meeting.ID });
}


[HttpPost]
public IActionResult EditMeeting(int id, string title, string place,DateTime meetingDateTime)
{
    if (id <= 0 || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(place) || meetingDateTime == default)
    {
        return Json(new { success = false, message = "输入数据无效" });
    }

    var meeting = _context.Meetings.FirstOrDefault(m => m.ID == id);
    if (meeting == null)
    {
        return Json(new { success = false, message = "找不到会议" });
    }

    // 更新会议数据
    meeting.Title = title;
    meeting.Place = place;
    meeting.MeetingDateTime = meetingDateTime;

    _context.SaveChanges();

    return Json(new { success = true });
}


[HttpPost]
public IActionResult DeleteMeeting(int meetingId)
{
    var meeting = _context.Meetings
        .Include(m => m.Sessionusers) // 確保載入相關 Sessionusers
        .FirstOrDefault(m => m.ID == meetingId);

    if (meeting == null)
    {
        return Json(new { success = false, message = "找不到指定的會議" });
    }

    _context.Sessionusers.RemoveRange(meeting.Sessionusers); // 刪除相關 Sessionusers
    _context.Meetings.Remove(meeting); // 刪除會議
    _context.SaveChanges();

    return Json(new { success = true });
}



[HttpGet]
public IActionResult Edit(int id)
{
    var sessionuser = _context.Sessionusers.FirstOrDefault(u => u.ID == id);
    if (sessionuser == null)
    {
        return NotFound();
    }
    return View(sessionuser);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(Sessionuser sessionuser)
{
    if (sessionuser == null || sessionuser.ID <= 0)
    {
        return BadRequest("無效的 Sessionuser 資料");
    }

    if (ModelState.IsValid)
    {
        try
        {
            // 更新 Sessionuser 資料
            var existingUser = _context.Sessionusers.FirstOrDefault(u => u.ID == sessionuser.ID);
            if (existingUser != null)
            {
                existingUser.FirstName = sessionuser.FirstName;
                existingUser.LastName = sessionuser.LastName;
                existingUser.ChineseName = sessionuser.ChineseName;
                existingUser.Email = sessionuser.Email;
                existingUser.City = sessionuser.City;
                existingUser.Country = sessionuser.Country;
                existingUser.IdentityType1 = sessionuser.IdentityType1;
                existingUser.IdentityType2 = sessionuser.IdentityType2;
                existingUser.IDNumber = sessionuser.IDNumber;
                existingUser.MeetingID = sessionuser.MeetingID;
                existingUser.Remark = sessionuser.Remark;
                existingUser.Phone = sessionuser.Phone;
                existingUser.Speaker = sessionuser.Speaker;

                // 更新 Joinlist 資料
                var joinlist = _context.Joinlists.FirstOrDefault(j => j.RegNo == sessionuser.RegNo);
                if (joinlist != null)
                {
                    joinlist.FirstName = sessionuser.FirstName;
                    joinlist.LastName = sessionuser.LastName;
                    joinlist.ChineseName = sessionuser.ChineseName;
                    joinlist.Email = sessionuser.Email;
                    joinlist.City = sessionuser.City;
                    joinlist.Country = sessionuser.Country;
                    joinlist.IdentityType1 = sessionuser.IdentityType1;
                    joinlist.IdentityType2 = sessionuser.IdentityType2;
                    joinlist.IDNumber = sessionuser.IDNumber;
                    joinlist.Remark = sessionuser.Remark;
                    joinlist.Speaker = sessionuser.Speaker;
                    joinlist.Phone = sessionuser.Phone;


                    _context.Joinlists.Update(joinlist);
                }
                else
                {
                    _logger.LogWarning($"未找到匹配的 Joinlist，RegNo: {sessionuser.RegNo}");
                }
                _context.SaveChanges();
                return RedirectToAction("Sessionuser");
            }

            return NotFound("找不到對應的 Sessionuser 資料");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新 Sessionuser 發生錯誤: {ex.Message}");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    return View(sessionuser);
}

[HttpGet]
public JsonResult GetMeetingStats(int meetingId)
{
    var totalUsers = _context.Sessionusers.Count(u => u.MeetingID == meetingId);
    var checkedInUsers = _context.Sessionusers.Count(u => u.MeetingID == meetingId && u.RegistrationStatus);

    return Json(new { totalUsers, checkedInUsers });
}



[HttpPost]
public IActionResult ToggleCheckInStatus(int id, bool status)
{
    // 取得 Sessionuser
    var sessionuser = _context.Sessionusers.FirstOrDefault(u => u.ID == id);
    if (sessionuser == null)
    {
        return NotFound("找不到該使用者");
    }

    // 取得對應的 Joinlist
    var joinlist = _context.Joinlists.FirstOrDefault(j => j.RegNo == sessionuser.RegNo);

    // 檢查是否真的有變更
    if (sessionuser.RegistrationStatus == status && (joinlist == null || joinlist.RegistrationStatus == status))
    {
        _logger.LogWarning($"沒有需要更新的變更，Sessionuser ID: {sessionuser.ID}");
        return RedirectToAction("Edit", new { id = id });
    }

    // ✅ 更新 Sessionuser
    sessionuser.RegistrationStatus = status;
    _context.Sessionusers.Update(sessionuser);

    // ✅ 更新 Joinlist
    if (joinlist != null)
    {
        joinlist.RegistrationStatus = status;
        _context.Joinlists.Update(joinlist);
    }
    else
    {
        _logger.LogWarning($"未找到匹配的 Joinlist，RegNo: {sessionuser.RegNo}");
    }

    // ✅ 確保 `SaveChanges()` 成功
    try
    {
        int affectedRows = _context.SaveChanges();
        if (affectedRows == 0)
        {
            _logger.LogError("SaveChanges() 未成功執行，Sessionuser 可能未更新");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"資料庫更新失敗: {ex.Message}");
        return StatusCode(500, "伺服器內部錯誤");
    }

    TempData["StatusMessage"] = status ? "報到成功" : "報到已取消";
    return RedirectToAction("Edit", new { id = id });
}





                // 新增Joinlist的功能
[HttpPost]
public async Task<IActionResult> AddSessionuser(SessionuserViewModel model)
{
    if (model.Sessionuser.MeetingID == 0)
    {
        return BadRequest("請選擇會議後新增人員");
    }

    if (string.IsNullOrEmpty(model.Sessionuser.FirstName) ||
        string.IsNullOrEmpty(model.Sessionuser.LastName) ||
        string.IsNullOrEmpty(model.Sessionuser.Email) ||
        string.IsNullOrEmpty(model.Sessionuser.IDNumber))
    {
        return BadRequest("請填寫完整的姓名、Email 和身分證號碼");
    }

    // **檢查 Sessionusers 是否已有相同的身份證號碼**
    var existingUser = await _context.Sessionusers
        .FirstOrDefaultAsync(u => u.IDNumber == model.Sessionuser.IDNumber);

    if (existingUser != null)
    {
        return BadRequest("該身份證號碼已申請過，無法重複新增");
    }

    // 生成 Barcode
    string barcode = GenerateBarcode(model.Sessionuser.IDNumber);

    // 檢查 Barcode 是否已存在
    while (await _context.Joinlists.AnyAsync(j => j.Barcode == barcode))
    {
        barcode += new Random().Next(0, 9); // 避免重複，增加隨機數
    }

    model.Sessionuser.Barcode = barcode;
    model.Sessionuser.RegistrationStatus = false; // 預設為未報到

    if (string.IsNullOrEmpty(model.Sessionuser.Speaker))
    {
        model.Sessionuser.Speaker = "普通成員"; // 預設角色
    }

    var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.ID == model.Sessionuser.MeetingID);
    if (meeting == null)
    {
        return BadRequest("選擇的會議不存在");
    }

    // **同步更新 Joinlist**
    var existingJoinlistEntry = await _context.Joinlists.FirstOrDefaultAsync(j => j.IDNumber == model.Sessionuser.IDNumber);
    if (existingJoinlistEntry != null)
    {
        return BadRequest("該身份證號碼已申請過，無法重複新增");
    }

    string selectedPrefix = "ON";
    var maxRegNoEntry = await _context.Joinlists
        .Where(j => j.RegNo.StartsWith(selectedPrefix))
        .OrderByDescending(j => j.RegNo)
        .FirstOrDefaultAsync();

    int nextNumber = 1;
    if (maxRegNoEntry != null)
    {
        string numericPart = maxRegNoEntry.RegNo.Substring(selectedPrefix.Length);
        if (int.TryParse(numericPart, out int currentMax))
        {
            nextNumber = currentMax + 1;
        }
    }

    string newRegNo = $"{selectedPrefix}{nextNumber:D4}";

    var newJoinlistEntry = new Joinlist
    {
        RegNo = newRegNo,
        FirstName = model.Sessionuser.FirstName,
        LastName = model.Sessionuser.LastName,
        ChineseName = model.Sessionuser.ChineseName,
        Email = model.Sessionuser.Email,
        City = model.Sessionuser.City,
        Country = model.Sessionuser.Country,
        RegistrationStatus = model.Sessionuser.RegistrationStatus,
        IdentityType1 = model.Sessionuser.IdentityType1,
        IdentityType2 = model.Sessionuser.IdentityType2,
        IDNumber = model.Sessionuser.IDNumber,
        Barcode = barcode // 存入 Joinlist
    };

    _context.Joinlists.Add(newJoinlistEntry);
    model.Sessionuser.RegNo = newRegNo;

    _context.Sessionusers.Add(model.Sessionuser);
    await _context.SaveChangesAsync();

    return Json(new { success = true });
}


// SHA256 加密 IDNumber 來生成 Barcode
private string GenerateBarcode(string idNumber)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(idNumber));
        StringBuilder hash = new StringBuilder();

        foreach (byte b in hashBytes)
        {
            hash.Append(b.ToString("x2")); // 轉換成 16 進制字串
        }

        return hash.ToString().Substring(0, 12).ToUpper(); // 取前 12 碼
    }
}

[HttpPost]
public async Task<IActionResult> AddSessionuregister(Sessionuser model)
{
    model.RegistrationStatus = false; // 預設報到狀態為 false
    model.Speaker = "普通成員"; // 🚀 這行確保新註冊者角色一定是 "普通成員"

    // 檢查 Joinlist 是否已有該 IDNumber 的記錄
    var existingJoinlistEntry = await _context.Joinlists
        .FirstOrDefaultAsync(j => j.IDNumber == model.IDNumber);

    if (existingJoinlistEntry != null)
    {
        model.RegNo = existingJoinlistEntry.RegNo;
    }
    else
    {
        string selectedPrefix = model.RegNo;
        var maxRegNoEntry = await _context.Joinlists
            .Where(j => j.RegNo.StartsWith(selectedPrefix))
            .OrderByDescending(j => j.RegNo)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (maxRegNoEntry != null)
        {
            string numericPart = maxRegNoEntry.RegNo.Substring(selectedPrefix.Length);
            if (int.TryParse(numericPart, out int currentMax))
            {
                nextNumber = currentMax + 1;
            }
        }

        model.RegNo = $"{selectedPrefix}{nextNumber:D4}";

        var newJoinlistEntry = new Joinlist
        {
            RegNo = model.RegNo,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ChineseName = model.ChineseName,
            Email = model.Email,
            City = model.City,
            Country = model.Country,
            RegistrationStatus = model.RegistrationStatus,
            IdentityType1 = model.IdentityType1,
            IdentityType2 = model.IdentityType2,
            IDNumber = model.IDNumber,
            Speaker = "普通成員"  // 🚀 同步設定 Joinlist 的 Speaker 也是 "普通成員"
        };

        _context.Joinlists.Add(newJoinlistEntry);
    }

    _context.Sessionusers.Add(model);
    await _context.SaveChangesAsync();

    return RedirectToAction("Sessionregister");
}

                // 生成唯一條碼的邏輯
        private async Task<string> GenerateUniqueBarcodeAsync()
        {
            Random random = new Random();
            while (true)
            {
                char letter = (char)random.Next('A', 'Z' + 1);
                string numbers = random.Next(1000000, 9999999).ToString("D7");
                string barcode = $"*{letter}{numbers}*";

                bool exists = await _context.Joinlists.AnyAsync(j => j.Barcode == barcode);
                if (!exists)
                {
                    return barcode;
                }
            }
        }

    

  

        // 显示 Sessionuser 页面
        [HttpGet]
        public async Task<IActionResult> Sessionuser(string searchColumn, string searchKeyword, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Sessionusers.AsQueryable();

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchKeyword))
            {
                query = searchColumn switch
                {
                    "regno" => query.Where(j => j.RegNo.Contains(searchKeyword)),
                    "firstname" => query.Where(j => j.FirstName.Contains(searchKeyword)),
                    "lastname" => query.Where(j => j.LastName.Contains(searchKeyword)),
                    "chinesename" => query.Where(j => j.ChineseName.Contains(searchKeyword)),
                    "country" => query.Where(j => j.Country.Contains(searchKeyword)),
                    "barcode" => query.Where(j => j.Barcode.Contains(searchKeyword)),
                    _ => query
                };
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var sessionusers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SessionuserViewModel
            {
                Sessionusers = sessionusers,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SearchSessionuser(string searchColumn, string searchKeyword, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Sessionusers.AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword) && searchColumn.ToLower() != "all")
            {
                switch (searchColumn)
                {
                    case "regno":
                        query = query.Where(j => j.RegNo.Contains(searchKeyword));
                        break;
                    case "firstname":
                        query = query.Where(j => j.FirstName.Contains(searchKeyword));
                        break;
                    case "lastname":
                        query = query.Where(j => j.LastName.Contains(searchKeyword));
                        break;
                    case "chinesename":
                        query = query.Where(j => j.ChineseName.Contains(searchKeyword));
                        break;
                    case "country":
                        query = query.Where(j => j.Country.Contains(searchKeyword));
                        break;
                    case "barcode":
                        query = query.Where(j => j.Barcode.Contains(searchKeyword));
                        break;
                }
            }

            query = query.OrderBy(j => j.ID);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var sessionusers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SessionuserViewModel
            {
                Sessionusers = sessionusers,
                CurrentPage = page,
                TotalPages = totalPages
            };

            var htmlString = await RenderPartialViewToString("_SessionuserTablePartial", viewModel);

            return Json(new { tableHtml = htmlString, currentPage = page, totalPages = totalPages });
        }

        // Render Partial View to String Helper
        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
