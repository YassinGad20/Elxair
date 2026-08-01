using Elxair.Models;

namespace Elxair.ViewModels
{
    public class ProductDetailsVM
    {
        public Perfume Perfume { get; set; }

        public Dictionary<int, Promotion?> Promotions { get; set; }
            = new();

        public Dictionary<int, decimal> FinalPrices { get; set; }
            = new();

        public Dictionary<int, decimal> DiscountPercentages { get; set; }
            = new();
    }
}