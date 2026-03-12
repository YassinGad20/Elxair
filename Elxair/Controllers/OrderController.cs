using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class OrderController : Controller
    {
        OrderService os = new OrderService();
        public IActionResult CreateOrder(int userId)
        {
            os.CreateOrder(userId);
            return RedirectToAction("MyOrders" , new {userId = userId});
        }

        public IActionResult MyOrders(int userId)
        {
            List<Order> orders = os.GetUserOrders(userId);
            return View(orders);
        }
    }
}
