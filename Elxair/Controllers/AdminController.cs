using Elixir.Services;
using Elxair.Models;
using Elxair.Models.AI;
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
        private readonly BusinessAnalyticsService businessAnalyticsService;
        private readonly ReportService reportService;
        private readonly JsonReportService _jsonReportService;
        private readonly IWebHostEnvironment _environment;
        private readonly AiService _aiService;
        private readonly RagService ragService;

        public AdminController(
            AdminService adminService,
            PaymentService paymentService,
            ElxairContext context,
            AiService aiService,
            BusinessAnalyticsService businessAnalyticsService,
            RagService ragService,
            ReportService reportService,
            JsonReportService jsonReportService,
            IWebHostEnvironment environment)
        {
            this.adminService = adminService;
            this.paymentService = paymentService;
            this._context = context;
            this._aiService = aiService;
            this.businessAnalyticsService = businessAnalyticsService;
            this.ragService = ragService;
            this.reportService = reportService;

            _jsonReportService = jsonReportService;

            _environment = environment;
        }



        [HttpGet]
        public async Task<IActionResult> ProfitDashboard()
        {
            var results = await _aiService.PredictAllPerfumes(
                DateTime.Now.Month,
                DateTime.Now.Year);

            return View(results
                .OrderByDescending(x => x.PredictedProfit)
                .ToList());
        }

        public async Task<IActionResult> BusinessAnalytics()
        {
            var predictions = await _aiService.PredictAllPerfumes(
                DateTime.Now.Month,
                DateTime.Now.Year
                );

            var report = businessAnalyticsService.Analyze(predictions);

            return View(report);
        }

        public async Task<IActionResult> GenerateReport()
        {
            var predictions = await _aiService.PredictAllPerfumes(
                DateTime.Now.Month,
                DateTime.Now.Year);

            var report = businessAnalyticsService.Analyze(predictions);

            report.ReportMonth = DateTime.Now.Month;
            report.ReportYear = DateTime.Now.Year;

            string pdfPath =
                await reportService.SavePdfAsync(
                    report,
                    _environment);

            string jsonPath =
                await _jsonReportService.SaveJsonAsync(
                    report,
                    _environment);

            var reportEntity = new Report
            {
                ReportMonth = report.ReportMonth,
                ReportYear = report.ReportYear,
                GeneratedAt = DateTime.Now,

                TotalPredictedProfit = report.TotalPredictedProfit,
                TotalPredictedUnits = report.TotalPredictedUnits,

                PdfPath = pdfPath,
                JsonPath = jsonPath,

                GeneratedBy = "Admin",
                Status = "Completed"
            };

            _context.Reports.Add(reportEntity);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Report generated successfully.";

            return RedirectToAction(nameof(BusinessAnalytics));
        }

        public IActionResult ReportsHistory()
        {
            var reports = _context.Reports
                .OrderByDescending(r => r.GeneratedAt)
                .ToList();

            return View(reports);
        }

        public IActionResult DownloadPdf(int id)
        {
            var report = _context.Reports.Find(id);

            if (report == null)
                return NotFound();

            string filePath = Path.Combine(
                _environment.WebRootPath,
                report.PdfPath);

            return PhysicalFile(
                filePath,
                "application/pdf",
                Path.GetFileName(filePath));
        }

        public IActionResult DownloadJson(int id)
        {
            var report = _context.Reports.Find(id);

            if (report == null)
                return NotFound();

            string filePath = Path.Combine(
                _environment.WebRootPath,
                report.JsonPath);

            return PhysicalFile(
                filePath,
                "application/json",
                Path.GetFileName(filePath));
        }

        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound();

            string pdf = Path.Combine(
                _environment.WebRootPath,
                report.PdfPath);

            string json = Path.Combine(
                _environment.WebRootPath,
                report.JsonPath);

            if (System.IO.File.Exists(pdf))
                System.IO.File.Delete(pdf);

            if (System.IO.File.Exists(json))
                System.IO.File.Delete(json);

            _context.Reports.Remove(report);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ReportsHistory));
        }


        public IActionResult Chatbot()
        {
            return View();
        }


       [HttpPost]
    public async Task<IActionResult> AskAI([FromBody] ChatRequest request)
    {
        var answer = await ragService.AskAsync(request.Message);

        return Json(new
        {
            answer
        });
    }


    public IActionResult Insight()
        {
            // Prepare category data for the comparison chart

            var catrgoryData = _context.OrderItems
                .GroupBy(oi => oi.PerfumeSize.Perfume.Category.Name)
                .Select(g => new { Name = g.Key, count = g.Sum(oi => oi.Quantity) })
                .ToList();

            // Prepare perfume data for the comparison chart

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

            // Monthly data

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

            // Payment details
            var payment = _context.Payments
                .FirstOrDefault(p => p.OrderId == id);

            ViewBag.Payment = payment;

            return View(order);
        }
    }
}
