namespace Elxair.Models
{
    public class PredictionReport
    {
        public DateTime GeneratedAt { get; set; }

        public string ModelVersion { get; set; }

        public int PredictionMonth { get; set; }

        public int PredictionYear { get; set; }

        public int TotalProducts { get; set; }

        public List<PredictionResult> Predictions { get; set; } = new();
    }
}
