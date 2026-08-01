using Elxair.Models;
using Microsoft.AspNetCore.Mvc;
using Elxair.Services;
using Elxair.ViewModels;
public class CartController : Controller
{
    private readonly CartService cs;
    private readonly IPromotionService promotionService;

    public CartController(
        CartService cs,
        IPromotionService promotionService)
    {
        this.cs = cs;
        this.promotionService = promotionService;
    }

    public IActionResult Index()
    {
        var items = cs.GetUserCart();

        var vm = new CartVM
        {
            Items = items
        };

        foreach (var item in items)
        {
            var size = item.PerfumeSize;

            vm.Promotions[size.Id] = promotionService.GetActivePromotion(size);

            vm.FinalPrices[size.Id] = promotionService.GetDiscountedPrice(size);

            vm.DiscountPercentages[size.Id] =
                promotionService.GetDiscountPercentage(size);
        }

        return View(vm);
    }

    [HttpPost]
    public IActionResult AddToCart(int perfumeSizeId, int quantity = 1)
    {
        try
        {
            cs.AddToCart(perfumeSizeId, quantity);
            TempData["Success"] = "Added to cart successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int itemId)
    {
        try
        {
            cs.RemoveFromCart(itemId);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}