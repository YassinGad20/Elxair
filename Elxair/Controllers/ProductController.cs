using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class ProductController : Controller
    {
        ProductService ps = new ProductService();

        // عرض كل البرفانات للمستخدم
        public IActionResult Index(string? gender)
        {
            var perfumes = ps.GetAllPerfumes();

            if (!string.IsNullOrEmpty(gender))
            {
                perfumes = perfumes
                    .Where(p => p.Gender == gender)
                    .ToList();
            }

            return View(perfumes);
        }
        // تفاصيل البرفان + الأحجام
        public IActionResult Details(int id)
        {
            var perfume = ps.GetPerfume(id);

            if (perfume == null)
                return NotFound();

            return View(perfume);
        }
    }
}