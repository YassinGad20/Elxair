using System.Text.Json.Serialization;

namespace Elxair.Models
{
    public class PredictionRequest
    {
        [JsonPropertyName("Size")]
        public string Size { get; set; }

        [JsonPropertyName("Sold In Season")]
        public string SoldInSeason { get; set; }

        [JsonPropertyName("Perfume Demand")]
        public int PerfumeDemand { get; set; }

        [JsonPropertyName("Unit Price")]
        public float UnitPrice { get; set; }

        [JsonPropertyName("Gender")]
        public string Gender { get; set; }

        [JsonPropertyName("Category")]
        public string Category { get; set; }

        [JsonPropertyName("Perfume Season")] // ضيف ده عشان الموديل ميدي مش Error
        public string PerfumeSeason { get; set; } = "Winter";
    }

    public class PredictionResponse
    {
        public decimal PredictedProfit { get; set; }
    }
}