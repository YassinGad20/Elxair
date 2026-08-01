using Elxair.Models;
using System.ComponentModel.DataAnnotations;

namespace Elxair.ViewModels
{
    public class PromotionCreateVM
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public PromotionType PromotionType { get; set; }

        [Range(0.01, 999999)]
        public decimal Value { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public List<int> SelectedPerfumeSizeIds { get; set; } = new();

        public List<Perfume> Perfumes { get; set; } = new();
    }
}