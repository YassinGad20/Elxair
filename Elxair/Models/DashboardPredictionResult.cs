namespace Elxair.Models
{
    public class DashboardPredictionResult
    {
        public int PerfumeId { get; set; }

        public int PerfumeSizeId { get; set; }

        public string PerfumeName { get; set; }

        public string CategoryName { get; set; }

        public string Gender { get; set; }

        public string Size { get; set; }

        public int PredictedQuantity { get; set; }

        public decimal PredictedProfit { get; set; }
    }
}
