public class PredictionRequest
{

    public int PerfumeId { get; set; }

    public int PerfumeSizeId { get; set; }

    public string PerfumeName { get; set; }

    public string Month { get; set; }

    public string BottleSize { get; set; }

    public int BottleVolume { get; set; }

    public string Category { get; set; }

    public string Gender { get; set; }

    public string Season { get; set; }

    public int SoldInSeason { get; set; }

    public string PriceCategory { get; set; }

    public int NumberOfTransactions { get; set; }

    public double AvgDiscount { get; set; }
    public double AvgMarginProfit { get; set; }
}