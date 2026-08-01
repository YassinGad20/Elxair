using Elxair.Models;

namespace Elxair.ViewModels
{
    public class CartVM
    {
        public List<CartItem> Items { get; set; } = new();

        public Dictionary<int, Promotion?> Promotions { get; set; } = new();

        public Dictionary<int, decimal> FinalPrices { get; set; } = new();

        public Dictionary<int, decimal> DiscountPercentages { get; set; } = new();
    }
}