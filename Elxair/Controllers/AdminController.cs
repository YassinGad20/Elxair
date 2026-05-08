using Elxair.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Elxair.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService adminService;
        private readonly PaymentService paymentService;

        public AdminController(AdminService adminService, PaymentService paymentService)
        {
            this.adminService = adminService;
            this.paymentService = paymentService;
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
    }
}