using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Elxair.Models;

namespace Elxair.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    ProductService ps = new ProductService();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var perfumes = ps.GetAllPerfumes();

        ViewBag.Bestsellers = perfumes.Take(4).ToList();
        ViewBag.ForHim = perfumes.Where(p => p.Gender == "Him").Take(4).ToList();
        ViewBag.ForHer = perfumes.Where(p => p.Gender == "Her").Take(4).ToList();
        ViewBag.Unisex = perfumes.Where(p => p.Gender == "Unisex").Take(4).ToList();

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
}