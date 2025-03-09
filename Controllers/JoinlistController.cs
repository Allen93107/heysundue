using Microsoft.AspNetCore.Mvc.Rendering;
using Heysundue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Heysundue.Controllers
{
    public class JoinlistController : Controller
    {
        private readonly ArticleContext _context;
        private readonly ILogger<JoinlistController> _logger;
        private readonly ICompositeViewEngine _viewEngine; // 注入ViewEngine來渲染部分視圖
        private readonly IServiceProvider _serviceProvider; // 注入ServiceProvider

        public JoinlistController(ArticleContext context, ILogger<JoinlistController> logger, ICompositeViewEngine viewEngine, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
        }

[HttpPost]
public IActionResult AddJoinlist(Joinlist joinlist)
{
    if (joinlist == null)
    {
        return BadRequest("無效的 Joinlist 資料");
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Joinlists.Add(joinlist);
            _context.SaveChanges();
            return RedirectToAction("Joinlist2"); // 返回到 Joinlist2 頁面
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增 Joinlist 發生錯誤: {ex.Message}");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    return View("Joinlist2", new Joinlist2ViewModel
    {
        Joinlists = _context.Joinlists.ToList(),
        CurrentPage = 1,
        TotalPages = 1 // 根據需要設置正確的分頁信息
    });
}



[HttpGet]
public IActionResult Edit(int id)
{
    var joinlist = _context.Joinlists.FirstOrDefault(j => j.ID == id);
    if (joinlist == null)
    {
        return NotFound("找不到指定的 Joinlist 資料");
    }
    return View(joinlist);
}




[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(Joinlist joinlist)
{
    if (joinlist == null || joinlist.ID <= 0)
    {
        return BadRequest("無效的 Joinlist 資料");
    }

    if (ModelState.IsValid)
    {
        try
        {
            var existingJoinlist = _context.Joinlists.FirstOrDefault(j => j.ID == joinlist.ID);
            if (existingJoinlist != null)
            {
                existingJoinlist.FirstName = joinlist.FirstName;
                existingJoinlist.LastName = joinlist.LastName;
                existingJoinlist.ChineseName = joinlist.ChineseName;
                existingJoinlist.Email = joinlist.Email;
                existingJoinlist.City = joinlist.City;
                existingJoinlist.Country = joinlist.Country;
                existingJoinlist.IdentityType1 = joinlist.IdentityType1;
                existingJoinlist.IdentityType2 = joinlist.IdentityType2;
                existingJoinlist.IDNumber = joinlist.IDNumber;
                existingJoinlist.Remark = joinlist.Remark;
                existingJoinlist.Speaker = joinlist.Speaker;

                // 同步更新對應的 Sessionuser 資料
                var sessionuser = _context.Sessionusers.FirstOrDefault(s => s.RegNo == joinlist.RegNo);
                if (sessionuser != null)
                {
                    sessionuser.FirstName = joinlist.FirstName;
                    sessionuser.LastName = joinlist.LastName;
                    sessionuser.ChineseName = joinlist.ChineseName;
                    sessionuser.Email = joinlist.Email;
                    sessionuser.City = joinlist.City;
                    sessionuser.Country = joinlist.Country;
                    sessionuser.IdentityType1 = joinlist.IdentityType1;
                    sessionuser.IdentityType2 = joinlist.IdentityType2;
                    sessionuser.IDNumber = joinlist.IDNumber;
                    sessionuser.Remark = joinlist.Remark;
                    sessionuser.Speaker = joinlist.Speaker;
                }

                _context.SaveChanges();
                return RedirectToAction("Joinlist2");
            }

            return NotFound("找不到對應的 Joinlist 資料");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新 Joinlist 發生錯誤: {ex.Message}");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    return View(joinlist);
}

[HttpPost]
public async Task<IActionResult> EditJoinlist(Joinlist joinlist)
{
    if (joinlist == null || joinlist.ID <= 0)
    {
        return BadRequest("無效的 Joinlist 資料");
    }

    if (ModelState.IsValid)
    {
        try
        {
            var existingJoinlist = await _context.Joinlists.FindAsync(joinlist.ID);
            if (existingJoinlist != null)
            {
                existingJoinlist.FirstName = joinlist.FirstName;
                existingJoinlist.LastName = joinlist.LastName;
                existingJoinlist.ChineseName = joinlist.ChineseName;
                existingJoinlist.Email = joinlist.Email;
                existingJoinlist.City = joinlist.City;
                existingJoinlist.Country = joinlist.Country;
                existingJoinlist.IdentityType1 = joinlist.IdentityType1;
                existingJoinlist.IdentityType2 = joinlist.IdentityType2;
                existingJoinlist.IDNumber = joinlist.IDNumber;
                existingJoinlist.Remark = joinlist.Remark;
                existingJoinlist.Speaker = joinlist.Speaker;
                existingJoinlist.Phone = joinlist.Phone;

                // **更新 Barcode**
                existingJoinlist.Barcode = GenerateBarcode(joinlist.IDNumber);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Joinlist 資料更新成功，ID: {ID}", joinlist.ID);

                return RedirectToAction("Joinlist2");
            }

            return NotFound("找不到對應的 Joinlist 資料");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新 Joinlist 發生錯誤: {ex.Message}");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    return View(joinlist);
}

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


        public async Task<IActionResult> Joinlist2(string searchColumn, string searchKeyword, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Joinlists.AsQueryable();

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

            var joinlists = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new Joinlist2ViewModel
            {
                Joinlists = joinlists,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
[HttpPost]
public async Task<IActionResult> SearchJoinlist(string searchKeyword, int page = 1)
{
    int pageSize = 10;
    var query = _context.Joinlists.AsQueryable();

    if (!string.IsNullOrEmpty(searchKeyword))
    {
        query = query.Where(j =>
            j.RegNo.Contains(searchKeyword) ||
            j.FirstName.Contains(searchKeyword) ||
            j.LastName.Contains(searchKeyword) ||
            j.ChineseName.Contains(searchKeyword) ||
            j.Country.Contains(searchKeyword) ||
            j.Barcode.Contains(searchKeyword) ||
            j.Email.Contains(searchKeyword) ||
            j.City.Contains(searchKeyword) ||
            j.IdentityType1.Contains(searchKeyword) ||
            j.IdentityType2.Contains(searchKeyword) ||
            j.IDNumber.Contains(searchKeyword)
        );
    }

    query = query.OrderBy(j => j.ID);

    var totalRecords = await query.CountAsync();
    var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

    var joinlists = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var viewModel = new Joinlist2ViewModel
    {
        Joinlists = joinlists,
        CurrentPage = page,
        TotalPages = totalPages
    };

    return View("Joinlist2", viewModel);
}


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

        [HttpPost]
        public IActionResult UpdateJoinlist([FromBody] Joinlist model)
        {
            var joinlist = _context.Joinlists.Find(model.ID);
            if (joinlist != null)
            {
                joinlist.RegNo = model.RegNo;
                joinlist.FirstName = model.FirstName;
                joinlist.LastName = model.LastName;
                joinlist.ChineseName = model.ChineseName;
                joinlist.Country = model.Country;
                joinlist.RegistrationStatus = model.RegistrationStatus;
                joinlist.Barcode = model.Barcode;
                joinlist.Email = model.Email;
                joinlist.City = model.City;
                joinlist.IdentityType1 = model.IdentityType1;
                joinlist.IdentityType2 = model.IdentityType2;
                joinlist.IDNumber = model.IDNumber;

                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "資料更新失敗" });
        }

        [HttpPost]
        public IActionResult SaveChanges([FromBody] List<Joinlist> updates)
        {
            if (updates == null || !updates.Any())
            {
                Console.WriteLine("無資料更新");
                return Json(new { success = false, message = "無資料更新" });
            }

            foreach (var update in updates)
            {
                Console.WriteLine($"Updating record ID: {update.ID}");

                var existingJoinlist = _context.Joinlists.Find(update.ID);
                if (existingJoinlist != null)
                {
                    existingJoinlist.RegNo = update.RegNo;
                    existingJoinlist.FirstName = update.FirstName;
                    existingJoinlist.LastName = update.LastName;
                    existingJoinlist.ChineseName = update.ChineseName;
                    existingJoinlist.Country = update.Country;
                    existingJoinlist.RegistrationStatus = update.RegistrationStatus;
                    existingJoinlist.Barcode = update.Barcode;
                    existingJoinlist.Email = update.Email;
                    existingJoinlist.City = update.City;
                    existingJoinlist.IdentityType1 = update.IdentityType1;
                    existingJoinlist.IdentityType2 = update.IdentityType2;
                    existingJoinlist.IDNumber = update.IDNumber;

                    _context.Joinlists.Update(existingJoinlist);
                }
                else
                {
                    Console.WriteLine($"Record with ID {update.ID} not found.");
                }
            }

            try
            {
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving changes: " + ex.Message);
                return Json(new { success = false, message = "儲存至資料庫時發生錯誤" });
            }
        }
    }
}
