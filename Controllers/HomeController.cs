using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Heysundue.Models;

namespace Heysundue.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Banner()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult SetDate()
    {
        return View();
    }

    public IActionResult Registration()
    {
        return View();
    }

    public IActionResult Joinlist()
    {
        return View();
    }

    public IActionResult Accessdoor()
    {
        return View();
    }

    public IActionResult Doorsystem()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
