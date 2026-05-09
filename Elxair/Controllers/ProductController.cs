using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ضفنا دي عشان الـ Async تشتغل
using Elxair.Models;

namespace Elxair.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService ps;
        private readonly ElxairContext context; // يفضل تسيبها كدة عشان الـ Index والـ Details شغالين بيها

        public ProductController(ProductService ps, ElxairContext context)
        {
            this.ps = ps;
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int perfumeId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Please login first" });

            // غيرنا _context لـ context عشان تطابق التعريف اللي فوق
            var existing = await context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PerfumeId == perfumeId);

            if (existing != null)
            {
                context.Favorites.Remove(existing);
                await context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false });
            }
            else
            {
                context.Favorites.Add(new Favorite { UserId = userId, PerfumeId = perfumeId });
                await context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true });
            }
        }

        public async Task<IActionResult> Favorites()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var favoriteProducts = await context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Perfume)
                    .ThenInclude(p => p.Category)
                .Include(f => f.Perfume)        // ← ضيف دي
                    .ThenInclude(p => p.Sizes)  // ← والـ Sizes
                .Select(f => f.Perfume)
                .ToListAsync();

            return View(favoriteProducts);
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