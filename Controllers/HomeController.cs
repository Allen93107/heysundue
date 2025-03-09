using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Heysundue.Models;
using Heysundue.Views.Home;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace Heysundue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ArticleContext _context;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ArticleContext context , ICompositeViewEngine viewEngine, IServiceProvider serviceProvider, ILogger<HomeController> logger)
        {
            _context = context;
        _viewEngine = viewEngine;  // 将参数正确赋值给字段
        _serviceProvider = serviceProvider;  // 将参数正确赋值给字段
        _logger = logger;
        }        
[HttpGet]
public async Task<IActionResult> CheckUpcomingMeetings()
{
    var currentTime = DateTime.Now;

    var upcomingMeetings = await _context.Meetings
        .Where(m => m.MeetingDateTime >= currentTime && m.MeetingDateTime <= currentTime.AddMinutes(30))
        .Select(m => new
        {
            Title = m.Title,
            MeetingDateTime = m.MeetingDateTime,
            Place = m.Place, // 確認這裡是否有正確傳遞
            Speaker = m.Sessionusers
                .Where(s => s.Speaker != null && s.MeetingID == m.ID)
                .Select(s => new
                {
                    ChineseName = s.ChineseName,
                    Phone = s.Phone
                })
                .FirstOrDefault()
        })
        .ToListAsync();

    _logger.LogInformation("Upcoming Meetings Data: {0}", JsonSerializer.Serialize(upcomingMeetings));

    return Json(upcomingMeetings);
}
[HttpPost]
public IActionResult UpdateSpeakersForYesterdayEndedMeetings()
{
    var yesterday = DateTime.Now.AddDays(-1).Date;

    // 查詢昨天結束的會議
    var endedMeetings = _context.Meetings
        .Where(m => m.MeetingDateTime.Date == yesterday)
        .ToList();

    if (!endedMeetings.Any())
    {
        return Json(new { success = false, message = "昨天沒有已結束的會議。" });
    }

    foreach (var meeting in endedMeetings)
    {
        // 更新會議成員 speaker 為 "普通成員"
        var sessionUsers = _context.Sessionusers
            .Where(s => s.MeetingID == meeting.ID && s.Speaker != "普通成員")
            .ToList();

        foreach (var user in sessionUsers)
        {
            user.Speaker = "普通成員";
        }
    }

    _context.SaveChanges();

    return Json(new { success = true, message = "昨天結束的會議成員已更新為普通成員。" });
}

[HttpPost]
public IActionResult UpdateSpeakersForMeeting(int meetingId)
{
    var meeting = _context.Meetings.FirstOrDefault(m => m.ID == meetingId);
    if (meeting == null)
    {
        return Json(new { success = false, message = "會議未找到。" });
    }

    var sessionUsers = _context.Sessionusers
        .Where(s => s.MeetingID == meetingId && s.Speaker != "普通成員")
        .ToList();

    if (!sessionUsers.Any())
    {
        return Json(new { success = false, message = "會議成員無需更新。" });
    }

    foreach (var user in sessionUsers)
    {
        user.Speaker = "普通成員";
    }

    _context.SaveChanges();

    return Json(new { success = true, message = $"會議 (ID: {meetingId}) 的成員已更新為普通成員。" });
}



    [HttpGet]
    public IActionResult Logout()
    {
        // 将用户角色状态设置为 Guest
        HttpContext.Session.SetString("UserRole", "Guest");

        // 重定向到主页或登录页面
        return RedirectToAction("Login2", "Home");
    }

[HttpGet]
public async Task<IActionResult> Userlist2(int page = 1)
{
    int pageSize = 3;  // 每頁顯示 3 條記錄

    var query = _context.Members.AsQueryable();
    int totalRecords = await query.CountAsync();
    int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

    var members = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var model = new Userlist2Model
    {
        Members = members,
        CurrentPage = page,
        TotalPages = totalPages,
        LevelOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "Root", Text = "Root" },
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Member", Text = "Member" }
        }
    };

    return View(model);
}


    
[HttpPost]
public async Task<IActionResult> UpdateUser(int memberId)
{
    var member = await _context.Members.FindAsync(memberId);
    if (member == null)
    {
        return NotFound();
    }

    // 通过 Request.Form 读取每个字段的值
    var userName = Request.Form[$"userName_{memberId}"];
    var userPassword = Request.Form[$"userPassword_{memberId}"];
    var phone = Request.Form[$"phone_{memberId}"];
    var email = Request.Form[$"email_{memberId}"];
    var level = Request.Form[$"level_{memberId}"];

    // 更新用户信息
    member.UserName = userName;
    member.UserPassword = userPassword;
    member.Phone = int.Parse(phone); // 确保 phone 是 int 类型
    member.Email = email;
    member.Level = level;

    await _context.SaveChangesAsync();

    return RedirectToAction("Userlist2");
}


        [HttpPost]
        public IActionResult SetIcon(string selectedIcon)
        {
            // 将用户选择的图标存储到 Session 中
            HttpContext.Session.SetString("Favicon", selectedIcon);

            // 重定向回 Banner2 页面
            return RedirectToAction("Banner2");
        }
    [HttpPost]
    public async Task<IActionResult> SearchUserlist2(string searchColumn, string searchKeyword, DateTime? searchDate, int page = 1)
    {
    int pageSize = 3;  // 每頁顯示 10 條記錄
    var query = _context.Members.AsQueryable();

    // 日期篩選
    if (searchDate.HasValue)
    {
        query = query.Where(m => m.Date >= searchDate.Value);
    }

    // 搜尋欄位和關鍵字篩選
    if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchKeyword))
    {
        switch (searchColumn.ToLower())
        {
            case "username":
                query = query.Where(m => m.UserName.Contains(searchKeyword));
                break;
            case "phone":
                query = query.Where(m => m.Phone.ToString().Contains(searchKeyword));
                break;
            case "email":
                query = query.Where(m => m.Email.Contains(searchKeyword));
                break;
            case "level":
                query = query.Where(m => m.Level.Contains(searchKeyword));
                break;
        }
    }

    // 計算總頁數
    int totalRecords = await query.CountAsync();
    int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

    // 取得當前頁的數據
    var members = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // 構建模型
    var model = new Userlist2Model
    {
        Members = members,
        CurrentPage = page,
        TotalPages = totalPages,
        LevelOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "Root", Text = "Root" },
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Member", Text = "Member" }
        }
    };

    // 返回部分視圖
    return PartialView("_MemberListPartial", model);
}


        
        public IActionResult Index2()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login2()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login2(Login login)
        {
            Console.WriteLine("OnPost 方法被调用");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "模型验证失败");
                Console.WriteLine("模型验证失败");
                return View(login);
            }

            Console.WriteLine("模型验证成功");

            // 查詢資料庫中的 Member 表，檢查是否有匹配的用戶名和密碼
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.UserName == login.Username && m.UserPassword == login.Password);

            if (member != null)
            {
                // 如果找到匹配的用戶，將 Level 存入 Session
                HttpContext.Session.SetString("UserRole", member.Level);
                Console.WriteLine("登录成功，用户角色：" + member.Level);
                return RedirectToAction("Index2");
            }

            // 如果未找到匹配的用戶，返回錯誤信息
            ModelState.AddModelError(string.Empty, "无效的登录尝试");
            Console.WriteLine("登录失败，用户名或密码错误。");
            return View(login);
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult SetDate()
        {
            return View();
        }

        public IActionResult SetDate2()
        {
            return View(new Article());
        }

        [HttpGet]
        public IActionResult SetDate3()
        {
            var model = new SetDate3ViewModel
            {
                Persons = _context.Persons.ToList()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult InputUser2()
        {
            var model = new InputUser2ViewModel
            {
                Members = _context.Members.ToList()
                
            };
            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Registration2()
        {
            var model = new Registration2ViewModel
            {
                Registrations = _context.Registrations.ToList()
            };
            return View(model);
        }


        public IActionResult Joinlist()
        {
            return View();
        }


        public IActionResult Doorsystem2(string searchColumn, string searchKeyword)
        {
            var model = new Doorsystem2ViewModel
            {
                Doorsystems = _context.Doorsystems.ToList()
            };
            return View(model);
        }

        public IActionResult Accessdoor()
        {
            return View();
        }

        public IActionResult Accessdoor2()
        {
            var model = new Accessdoor2ViewModel
            {
                Accessdoors = _context.Accessdoors.ToList(),
                UniqueDates = _context.Accessdoors.Select(a => a.StartDate).Distinct().ToList(),
                UniqueRooms = _context.Accessdoors.Select(a => a.Room).Distinct().ToList(),
                UniqueSessions = _context.Accessdoors.Select(a => a.Session).Distinct().ToList()
            };
            return View(model);
        }

                [HttpPost]
        public async Task<IActionResult> SearchAccessdoor(string searchColumn, string searchKeyword)
        {
            var query = _context.Accessdoors.AsQueryable();

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchKeyword))
            {
                switch (searchColumn.ToLower())
                {
                    case "startdate":
                        if (DateTime.TryParse(searchKeyword, out DateTime date))
                        {
                            query = query.Where(j => j.StartDate == date);
                        }
                        break;
                    case "room":
                        query = query.Where(j => j.Room.Contains(searchKeyword));
                        break;
                    case "session":
                        query = query.Where(j => j.Session.Contains(searchKeyword));
                        break;
                }
            }

        var result = await query.ToListAsync();
            return PartialView("_AccessdoorTablePartial", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccessdoors()
        {
            var result = await _context.Accessdoors.ToListAsync();
            return PartialView("_AccessdoorTablePartial", result);
        }

        public IActionResult Doorsystem()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> SetDate2([Bind("ID,Number,Title,Link,ReleaseDate,Gender,Count,Date,Time,Location")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index2");
            }
            return View(article);
        }


        [HttpPost]
        public async Task<IActionResult> Doorsystem2(Doorsystem2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Doorsystems.Add(model.Doorsystem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Doorsystem2");
            }
            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> Registration2(Registration2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Registrations.Add(model.Registration);
                await _context.SaveChangesAsync();
                return RedirectToAction("Registration2");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetDate3(SetDate3ViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Persons.Add(model.Person);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index2");
            }

            model.Persons = _context.Persons.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InputUser2(InputUser2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Member.Date = DateTime.Now;
                model.Member.Level = "Member";
                _context.Members.Add(model.Member);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index2");
            }

            model.Members = _context.Members.ToList();
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> Accessdoor2(Accessdoor2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Accessdoors.Add(model.Accessdoor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Accessdoor2");
            }

            return View(model);
        }

  

    }
}
