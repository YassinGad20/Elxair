namespace Elxair.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public int ReportMonth { get; set; }

        public int ReportYear { get; set; }

        public DateTime GeneratedAt { get; set; }

        public decimal TotalPredictedProfit { get; set; }

        public int TotalPredictedUnits { get; set; }

        public string PdfPath { get; set; }

        public string JsonPath { get; set; }

        public string GeneratedBy { get; set; }

        public string Status { get; set; } = "Completed";
    }
}
