using Elxair.Models;
using Elxair.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elxair.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ElxairContext _context;

        public PromotionController(ElxairContext context)
        {
            _context = context;
        }

        // ===================== Index =====================

        public IActionResult Index()
        {
            var promotions = _context.Promotions
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(promotions);
        }

        // ===================== Create (GET) =====================

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new PromotionCreateVM
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                IsActive = true,

                Perfumes = _context.Perfumes
                    .Include(p => p.Sizes)
                    .OrderBy(p => p.Name)
                    .ToList()
            };

            return View(vm);
        }

        // ===================== Create (POST) =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PromotionCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Perfumes = _context.Perfumes
                    .Include(p => p.Sizes)
                    .OrderBy(p => p.Name)
                    .ToList();

                return View(vm);
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var promotion = new Promotion
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    PromotionType = vm.PromotionType,
                    Value = vm.Value,
                    StartDate = vm.StartDate,
                    EndDate = vm.EndDate,
                    IsActive = vm.IsActive
                };

                _context.Promotions.Add(promotion);
                _context.SaveChanges();

                foreach (var sizeId in vm.SelectedPerfumeSizeIds)
                {
                    var exists = _context.PromotionPerfumeSizes
                        .Include(x => x.Promotion)
                        .Any(x =>
                            x.PerfumeSizeId == sizeId &&
                            x.Promotion.IsActive &&
                            vm.StartDate <= x.Promotion.EndDate &&
                            vm.EndDate >= x.Promotion.StartDate);

                    if (exists)
                    {
                        transaction.Rollback();

                        ModelState.AddModelError("", "One or more selected perfume sizes already have an active promotion.");

                        vm.Perfumes = _context.Perfumes
                            .Include(p => p.Sizes)
                            .OrderBy(p => p.Name)
                            .ToList();

                        return View(vm);
                    }

                    _context.PromotionPerfumeSizes.Add(new PromotionPerfumeSize
                    {
                        PromotionId = promotion.Id,
                        PerfumeSizeId = sizeId
                    });
                }

                _context.SaveChanges();

                transaction.Commit();

                TempData["Success"] = "Promotion created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                ModelState.AddModelError("", ex.Message);

                vm.Perfumes = _context.Perfumes
                    .Include(p => p.Sizes)
                    .OrderBy(p => p.Name)
                    .ToList();

                return View(vm);
            }
        }
    }
}