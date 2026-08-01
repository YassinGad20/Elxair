using System.ComponentModel.DataAnnotations;

namespace Elxair.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public PromotionType PromotionType { get; set; }

        [Range(0.01, 999999)]
        public decimal Value { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PromotionPerfumeSize> PromotionPerfumeSizes { get; set; }
            = new List<PromotionPerfumeSize>();
    }
}