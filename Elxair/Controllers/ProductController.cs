using Microsoft.AspNetCore.Mvc;
using Elxair.Models;

namespace Elxair.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService ps;
        private readonly ElxairContext context;

        public ProductController(ProductService ps, ElxairContext context)
        {
            this.ps = ps;
            this.context = context;
        }

        public IActionResult Index(string? search, string? gender, int? categoryId)
        {
            var products = ps.SearchAndFilter(search, gender, categoryId) ?? new List<Perfume>();

            ViewBag.Search = search ?? "";
            ViewBag.Gender = gender ?? "";
            ViewBag.CategoryId = categoryId;

            ViewBag.Categories = context.Categories.ToList();

            ViewBag.ResultCount = products.Count;

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var perfume = ps.GetPerfume(id);

            if (perfume == null)
            {
                return NotFound();
            }
            var recommendations = ps.GetRecommendations(perfume);

            ViewBag.Recommendations = recommendations;

            return View(perfume);
        }
    }
}