using System.ComponentModel.DataAnnotations.Schema;

namespace Elxair.Models
{
    public class PerfumeSize
    {
        public int Id { get; set; }

        public int PerfumeId { get; set; }
        public Perfume Perfume { get; set; }

        public string Size { get; set; }   // 50ml - 100ml - 200ml
        public decimal CostPerBottle { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }
        [NotMapped]
        public decimal FinalPrice { get; set; }

        [NotMapped]
        public decimal DiscountAmount { get; set; }

        [NotMapped]
        public bool HasPromotion { get; set; }

        [NotMapped]
        public decimal DiscountPercentage { get; set; }

        // Promotions
        public ICollection<PromotionPerfumeSize> PromotionPerfumeSizes { get; set; }
            = new List<PromotionPerfumeSize>();
    }
}