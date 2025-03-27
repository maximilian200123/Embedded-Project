using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GarbageCollectionApp.Models;

namespace GarbageCollectionApp.Controllers;

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
     
        // if login is successful, redirect to the Admin dashboard
        return RedirectToAction("Index", "Admin");
    }
}
