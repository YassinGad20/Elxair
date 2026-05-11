using Elxair.Models;
using Elxair.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Elxair.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService adminService;
        private readonly PaymentService paymentService;
        private readonly ElxairContext _context;
        private readonly AiService _aiService;

        public AdminController(AdminService adminService, PaymentService paymentService, ElxairContext context, AiService aiService)
        {
            this.adminService = adminService;
            this.paymentService = paymentService;
            this._context = context;
            this._aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> ProfitDashboard()
        {
            // Get All Perfumes

            var perfumes = _context.Perfumes
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .ToList();

            var chartData = new List<object>();

            foreach (var perfume in perfumes)
            {
                // Calc Demand For EveryOne

                int totalDemand = _context.OrderItems
                    .Where(oi => oi.PerfumeSize.PerfumeId == perfume.Id)
                    .Sum(oi => (int?)oi.Quantity) ?? 0;

                // Prepare Data For Ai Model

                var requestData = new PredictionRequest
                {
                    Gender = perfume.Gender,
                    Category = perfume.Category?.Name ?? "General",
                    Size = perfume.Sizes.FirstOrDefault()?.Size ?? "100ml",
                    UnitPrice = (float)(perfume.Sizes.FirstOrDefault()?.Price ?? 0),
                    PerfumeDemand = totalDemand,
                    SoldInSeason = "Winter"
                };

                // Call AI

                decimal profit = await _aiService.GetPredictionAsync(requestData);
                chartData.Add(new { Name = perfume.Name, Profit = profit });
            }
            return View(chartData);
        }

        public IActionResult Insight()
        {
            // Perpare Category For Compression

            var catrgoryData = _context.OrderItems
                .GroupBy(oi => oi.PerfumeSize.Perfume.Category.Name)
                .Select(g => new { Name = g.Key, count = g.Sum(oi => oi.Quantity) })
                .ToList();

            // Perpare Perfumes For Compression

            var perfumeData = _context.OrderItems
                .GroupBy(oi => oi.PerfumeSize.Perfume.Name)
                .Select(g => new PerfumeSalesDto
                {
                    Name = g.Key,
                    SalesCount = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(p => p.SalesCount)
                .ToList();

            // Monthly Data

            var viewModel = new AdminDashboard
            {
                TotalRevenue = _context.OrderItems.Sum(oi => oi.Quantity * oi.Price),
                TotalOrders = _context.Orders.Count(),
                CategoryNames = catrgoryData.Select(c => c.Name).ToList(),
                CategorySales = catrgoryData.Select(c => c.count).ToList(),
                PerfumeComparison = perfumeData,
            };
            return View(viewModel);
        }

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "Admin";

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult Products()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View(adminService.GetAllPerfumes());
        }

        public IActionResult CreatePerfume()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.Categories = new SelectList(adminService.GetAllCategories(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult CreatePerfume(Perfume perfume)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(perfume.Gender))
                ModelState.AddModelError("Gender", "Please select gender.");

            if (perfume.CategoryId <= 0)
                ModelState.AddModelError("CategoryId", "Please select category.");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(adminService.GetAllCategories(), "Id", "Name");
                return View(perfume);
            }

            adminService.AddPerfume(perfume);
            TempData["Success"] = "Perfume added successfully.";
            return RedirectToAction("Products");
        }

        public IActionResult EditPerfume(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var perfume = adminService.GetAllPerfumes().FirstOrDefault(p => p.Id == id);


            if (perfume == null)
                return NotFound();

            ViewBag.Categories = new SelectList(adminService.GetAllCategories(), "Id", "Name");
            return View(perfume);
        }

        [HttpPost]
        public IActionResult EditPerfume(Perfume perfume)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                adminService.UpdatePerfume(perfume);
                TempData["Success"] = "Perfume updated successfully.";
                return RedirectToAction("Products");
            }

            ViewBag.Categories = new SelectList(adminService.GetAllCategories(), "Id", "Name");
            return View(perfume);
        }

        public IActionResult DeletePerfume(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            adminService.DeletePerfume(id);
            TempData["Success"] = "Perfume deleted.";
            return RedirectToAction("Products");
        }

        public IActionResult AddPerfumeSize(int perfumeId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.PerfumeId = perfumeId;
            return View();
        }

        [HttpPost]
        public IActionResult AddPerfumeSize(int perfumeId, string size, decimal price, int stock)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            adminService.AddPerfumeSize(perfumeId, size, price, stock);
            TempData["Success"] = "Size added successfully.";
            return RedirectToAction("EditPerfume", new { id = perfumeId });
        }

        public IActionResult EditPerfumeSize(int sizeId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var perfumeSize = adminService.GetAllPerfumes()
                .SelectMany(p => p.Sizes)
                .FirstOrDefault(s => s.Id == sizeId);

            if (perfumeSize == null)
                return NotFound();

            return View(perfumeSize);
        }

        [HttpPost]
        public IActionResult EditPerfumeSize(PerfumeSize size)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            adminService.UpdatePerfumeSize(size);
            TempData["Success"] = "Size updated.";
            return RedirectToAction("EditPerfume", new { id = size.PerfumeId });
        }

        public IActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View(adminService.GetAllOrders());
        }

        public IActionResult Payments()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View(paymentService.GetAllPayments());
        }

        public IActionResult ConfirmPayment(int paymentId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            paymentService.UpdateStatus(paymentId, "Paid");
            TempData["Success"] = "Payment confirmed.";
            return RedirectToAction("Payments");
        }

        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            adminService.UpdateOrderStatus(orderId, status);
            TempData["Success"] = $"Order #{orderId} marked as {status}.";
            return RedirectToAction("Orders");
        }
        public IActionResult OrderDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.PerfumeSize)
                        .ThenInclude(ps => ps.Perfume)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            // Payment Detalis
            var payment = _context.Payments
                .FirstOrDefault(p => p.OrderId == id);

            ViewBag.Payment = payment;

            return View(order);
        }
    }
}