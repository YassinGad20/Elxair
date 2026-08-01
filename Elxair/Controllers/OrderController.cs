using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService os;
        private readonly PaymentService paymentService;
        private readonly UserService us;

        public OrderController(
            OrderService os,
            PaymentService paymentService,
            UserService us)
        {
            this.os = os;
            this.paymentService = paymentService;
            this.us = us;
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(
            string fullName,
            string phone,
            string governorate,
            string address)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(governorate) ||
                string.IsNullOrWhiteSpace(address))
            {
                TempData["Error"] = "Please fill in all delivery details.";
                return RedirectToAction("Checkout");
            }

            int userId = us.GetCurrentUserId();

            string customerName =
                HttpContext.Session.GetString("UserName") ?? fullName;

            int orderId = os.CreateOrder();

            if (orderId == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            var order = os.GetUserOrders()
                          .FirstOrDefault(o => o.Id == orderId);

            decimal amount = order?.TotalPrice ?? 0;

            paymentService.CreatePayment(
                orderId,
                userId,
                customerName,
                amount,
                fullName,
                phone,
                governorate,
                address);

            TempData["Success"] = "Order placed successfully!";

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var orders = os.GetUserOrders();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = os.GetUserOrders()
                          .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}