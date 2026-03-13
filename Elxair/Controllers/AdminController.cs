using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class AdminController : Controller
    {
        AdminService adminService = new AdminService();

        // لوحة التحكم
        public IActionResult Dashboard()
        {
            return View();
        }

        // عرض كل البرفانات
        public IActionResult Products()
        {
            var perfumes = adminService.GetAllPerfumes();
            return View(perfumes);
        }

        // صفحة إضافة برفان جديد
        public IActionResult CreatePerfume()
        {
            return View();
        }

        // حفظ البرفان الجديد
        [HttpPost]
        public IActionResult CreatePerfume(Perfume perfume)
        {
            if (ModelState.IsValid)
            {
                adminService.AddPerfume(perfume);
                return RedirectToAction("Products");
            }
            return View(perfume);
        }

        // صفحة تعديل برفان
        public IActionResult EditPerfume(int id)
        {
            var perfume = adminService.GetAllPerfumes()
                .FirstOrDefault(p => p.Id == id);

            if (perfume == null)
                return NotFound();

            return View(perfume);
        }

        // حفظ تعديل البرفان
        [HttpPost]
        public IActionResult EditPerfume(Perfume perfume)
        {
            if (ModelState.IsValid)
            {
                adminService.UpdatePerfume(perfume);
                return RedirectToAction("Products");
            }
            return View(perfume);
        }

        // حذف برفان
        public IActionResult DeletePerfume(int id)
        {
            adminService.DeletePerfume(id);
            return RedirectToAction("Products");
        }

        // إضافة حجم + سعر للبرفان
        public IActionResult AddPerfumeSize(int perfumeId)
        {
            ViewBag.PerfumeId = perfumeId;
            return View();
        }

        [HttpPost]
        public IActionResult AddPerfumeSize(int perfumeId, string size, decimal price, int stock)
        {
            adminService.AddPerfumeSize(perfumeId, size, price, stock);
            return RedirectToAction("EditPerfume", new { id = perfumeId });
        }

        // تعديل حجم البرفان
        public IActionResult EditPerfumeSize(int sizeId)
        {
            var perfumeSize = adminService.GetAllPerfumes() // ممكن تعمل فانكشن جديد للـ Size
                                .SelectMany(p => p.Sizes)
                                .FirstOrDefault(s => s.Id == sizeId);

            if (perfumeSize == null)
                return NotFound();

            return View(perfumeSize);
        }

        [HttpPost]
        public IActionResult EditPerfumeSize(PerfumeSize size)
        {
            adminService.UpdatePerfumeSize(size);
            return RedirectToAction("EditPerfume", new { id = size.PerfumeId });
        }

        // عرض كل الأوردرات
        public IActionResult Orders()
        {
            var orders = adminService.GetAllOrders();
            return View(orders);
        }

        // تحديث حالة الأوردر
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            adminService.UpdateOrderStatus(orderId, status);
            return RedirectToAction("Orders");
        }
    }
}