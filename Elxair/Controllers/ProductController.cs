using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ضفنا دي عشان الـ Async تشتغل
using Elxair.Models;
using Elxair.Services;
using Elxair.ViewModels;

namespace Elxair.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService ps;
        private readonly ReviewService rs;
        private readonly IPromotionService promotionService;

        private readonly ElxairContext context; // يفضل تسيبها كدة عشان الـ Index والـ Details شغالين بيها

        public ProductController(ProductService ps, ReviewService rs, IPromotionService promotionService, ElxairContext context)
        {
            this.ps = ps;
            this.rs = rs;
            this.promotionService = promotionService;
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
                .Include(f => f.Perfume)        
                    .ThenInclude(p => p.Sizes)  
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

            var vm = new ProductDetailsVM
            {
                Perfume = perfume
            };

            foreach (var size in perfume.Sizes)
            {
                vm.Promotions[size.Id] = promotionService.GetActivePromotion(size);
                vm.FinalPrices[size.Id] = promotionService.GetDiscountedPrice(size);
                vm.DiscountPercentages[size.Id] = promotionService.GetDiscountPercentage(size);
            }

            ViewBag.Recommendations = ps.GetRecommendations(perfume);
            ViewBag.Reviews = rs.GetPerfumeReview(id);
            ViewBag.AverageRating = rs.GetAverageRating(id);
            ViewBag.ReviewCount = rs.GetReviewCount(id);

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddReview(int perfumeId, int rating, string comment)
        {
            if (HttpContext.Session.GetString("IsGuest") == "true")
                return RedirectToAction("Login", "Account");

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!rs.HasPurchasedPerfume(userId.Value, perfumeId))
            {
                TempData["Error"] = "You can only review perfumes you have purchased.";
                return RedirectToAction("Details", new { id = perfumeId });
            }

            if (rs.HasReviewed(userId.Value, perfumeId))
            {
                TempData["Error"] = "You have already reviewed this perfume.";
                return RedirectToAction("Details", new { id = perfumeId });
            }

            Review review = new Review
            {
                UserId = userId.Value,
                PerfumeId = perfumeId,
                Rating = rating,
                Comment = comment
            };

            rs.AddReview(review);

            TempData["Success"] = "Review added successfully.";

            return RedirectToAction("Details", new { id = perfumeId });
        }
    }
}