using Elxair.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Elxair.Controllers
{
    public class CartController : Controller
    {
        CartService cs = new CartService();
        public IActionResult GetUserCart(int userId)
        {
            
            List<CartItem> cartItems = cs.GetUserCart(userId);
            return View(cartItems);
        }

        public IActionResult AddToCart(int userId , int perfumeId , int quantity)
        {
            cs.AddToCart(userId, perfumeId, quantity);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int itemId)
        {
            cs.RemoveFromCart(itemId);
            return RedirectToAction("Index");
        }
    }
}
