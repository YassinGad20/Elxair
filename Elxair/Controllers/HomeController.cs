using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Elxair.Models;

namespace Elxair.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProductService ps;
    private readonly ElxairContext _context;

    public HomeController(ILogger<HomeController> logger, ProductService ps, ElxairContext context)
    {
        _logger = logger;
        this.ps = ps;
        _context = context;
    }

    public IActionResult Index()
    {
        var perfumes = ps.GetAllPerfumes();

        // Bestsellers 
        var topSellingIds = _context.OrderItems
            .GroupBy(oi => oi.PerfumeSize.PerfumeId)
            .Select(g => new { PerfumeId = g.Key, Total = g.Sum(oi => oi.Quantity) })
            .OrderByDescending(x => x.Total)
            .Take(4)
            .Select(x => x.PerfumeId)
            .ToList();

        var bestsellers = perfumes
            .Where(p => topSellingIds.Contains(p.Id))
            .OrderBy(p => topSellingIds.IndexOf(p.Id))
            .ToList();

        ViewBag.Bestsellers = bestsellers.Any() ? bestsellers : perfumes.Take(4).ToList();

        ViewBag.ForHim = perfumes.Where(p => p.Gender == "Him").Take(4).ToList();
        ViewBag.ForHer = perfumes.Where(p => p.Gender == "Her").Take(4).ToList();
        ViewBag.Unisex = perfumes.Where(p => p.Gender == "Unisex").Take(4).ToList();

        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}