using System.ComponentModel.DataAnnotations;

namespace Elxair.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PerfumeId { get; set; }
        public Perfume Perfume { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}