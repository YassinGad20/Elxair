using System.ComponentModel.DataAnnotations.Schema;
namespace Elxair.Models
{
    public class Perfume
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<PerfumeSize> Sizes { get; set; } = new();
        public string Gender { get; set; }

        public PerfumeSeason Season { get; set; }


        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [NotMapped]
        public bool HasPromotion { get; set; }

        [NotMapped]
        public decimal DisplayPrice { get; set; }

        [NotMapped]
        public decimal? OldPrice { get; set; }

        [NotMapped]
        public decimal DiscountPercentage { get; set; }
    }
}
