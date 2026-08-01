using System.Collections.Generic;
using System.Linq;

namespace Elxair.Models
{
    // Demand trend for a size: Up / Down / Flat
    // Determined by comparing each size's demand to the average demand
    // across all sizes. If you later add an order date field, swap this
    // for a real period-over-period comparison in the controller.
   

    // One row = one perfume at one specific size, with its predicted profit and trend
    public class PerfumeSizeInsight
    {
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Demand { get; set; }
        public decimal PredictedProfit { get; set; }
        public TrendDirection Trend { get; set; }
        public double TrendDeltaPercent { get; set; }
    }

    // Passed to the ProfitDashboard view: all rows + aggregated stats
    public class ProfitDashboardViewModel
    {
        public List<PerfumeSizeInsight> Items { get; set; } = new();

        public decimal TotalPredictedProfit => Items.Sum(i => i.PredictedProfit);
        public int TotalUnitsSold => Items.Sum(i => i.Demand);
        public int UpCount => Items.Count(i => i.Trend == TrendDirection.Up);
        public int DownCount => Items.Count(i => i.Trend == TrendDirection.Down);
        public int FlatCount => Items.Count(i => i.Trend == TrendDirection.Flat);
        public double AverageDemand => Items.Count > 0 ? Items.Average(i => i.Demand) : 0;

        public PerfumeSizeInsight? TopPerformer =>
            Items.OrderByDescending(i => i.PredictedProfit).FirstOrDefault();

        public PerfumeSizeInsight? WeakestPerformer =>
            Items.OrderBy(i => i.PredictedProfit).FirstOrDefault();
    }
}
