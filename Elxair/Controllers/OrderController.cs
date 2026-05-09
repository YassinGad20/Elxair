using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService os;
        private readonly PaymentService paymentService;

        // constructor واحد بس بياخد الاتنين
        public OrderController(OrderService os, PaymentService paymentService)
        {
            this.os = os;
            this.paymentService = paymentService;
        }

        public IActionResult Checkout()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(string fullName, string phone,
                                         string governorate, string address)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(governorate) ||
                string.IsNullOrWhiteSpace(address))
            {
                TempData["Error"] = "Please fill in all delivery details.";
                return RedirectToAction("Checkout");
            }

            string customerName = HttpContext.Session.GetString("UserName") ?? fullName;
            int orderId = os.CreateOrder(userId.Value);

            if (orderId == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            var order = os.GetUserOrders(userId.Value)
                          .FirstOrDefault(o => o.Id == orderId);
            decimal amount = order?.TotalPrice ?? 0;

            paymentService.CreatePayment(orderId, userId.Value, customerName,
                             amount, fullName, phone, governorate, address);

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var orders = os.GetUserOrders(userId.Value);
            return View(orders);
        }
    }
}