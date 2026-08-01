namespace Elxair.Models
{
    public class PredictionReportRequest
    {
        public int PredictionMonth { get; set; }

        public int PredictionYear { get; set; }

        public List<PredictionItem> Items { get; set; } = new();
    }
}
