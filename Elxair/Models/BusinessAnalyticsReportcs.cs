namespace Elxair.Models
{
    public class BusinessAnalyticsReport
    {
        public int ReportMonth { get; set; }
        public int ReportYear { get; set; }

        public decimal TotalPredictedProfit { get; set; }
        public int TotalPredictedUnits { get; set; }
        public int TotalProductsCount { get; set; }

        public DashboardPredictionResult TopPerformer { get; set; }
        public DashboardPredictionResult LowestPerformer { get; set; }

        public List<DashboardPredictionResult> Top10Products { get; set; }

        public List<CategoryAnalytics> Categories { get; set; }

        public List<GenderAnalytics> Genders { get; set; }

        public List<SizeAnalytics> Sizes { get; set; }
    }
}