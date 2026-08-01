namespace Elxair.Models
{
    public class PromotionPerfumeSize
    {
        public int Id { get; set; }

        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; }

        public int PerfumeSizeId { get; set; }
        public PerfumeSize PerfumeSize { get; set; }
    }
}