using Elxair.Models;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    CartService cs = new CartService();

    public IActionResult Index()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var cartItems = cs.GetUserCart(userId.Value);
        var userCart = new Cart { UserId = userId.Value, Items = cartItems };
        return View(userCart);
    }

    [HttpPost]
    public IActionResult AddToCart(int perfumeSizeId, int quantity = 1)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        try
        {
            cs.AddToCart(userId.Value, perfumeSizeId, quantity);
            TempData["Success"] = "Added to cart successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int itemId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        cs.RemoveFromCart(itemId);
        return RedirectToAction("Index");
    }
}