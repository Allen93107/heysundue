using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Heysundue.Controllers
{
    public class BannerController : Controller
    {
        [HttpPost]
        public IActionResult SetBanner(string selectedImage)
        {
            // 將用戶選擇的 Banner 圖片存儲到 Session 中
            HttpContext.Session.SetString("Favicon", selectedImage);
            TempData["ActionResult"] = "成功設置 Banner！";
            return RedirectToAction("Banner2");
        }

        [HttpPost]
        public IActionResult SetMap(string selectedImage)
        {
            // 將用戶選擇的地圖圖片存儲到 Session 中
            HttpContext.Session.SetString("Map", selectedImage);
            TempData["ActionResult"] = "成功設置地圖！";
            return RedirectToAction("Banner2");
        }

        [HttpGet]
        public IActionResult Banner2()
        {
            return View();
        }
    }
}
