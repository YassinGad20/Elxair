using Elxair.Models;
namespace Elxair.Services
{
    public class BusinessAnalyticsService
    {
        public BusinessAnalyticsReport Analyze(List<DashboardPredictionResult> predictions)
        {
            var report = new BusinessAnalyticsReport();
            report.TotalPredictedProfit = predictions.Sum(x => x.PredictedProfit);
            report.TotalPredictedUnits = predictions.Sum(x => x.PredictedQuantity);
            report.TotalProductsCount = predictions.Count;
            report.TopPerformer = predictions
                .OrderByDescending(x => x.PredictedProfit)
                .FirstOrDefault();
            report.LowestPerformer = predictions
                .OrderBy(x => x.PredictedProfit)
                .FirstOrDefault();
            report.Top10Products = predictions
                .OrderByDescending(x => x.PredictedProfit)
                .Take(10)
                .ToList();
            report.Categories = predictions
                .GroupBy(x => x.CategoryName)
                .Select(g => new CategoryAnalytics
                {
                    CategoryName = g.Key,
                    TotalUnits = g.Sum(x => x.PredictedQuantity),
                    TotalProfit = g.Sum(x => x.PredictedProfit)
                })

                .OrderByDescending(x => x.TotalProfit)
                .ToList();
            report.Genders = predictions
                .GroupBy(x => x.Gender)
                .Select(g => new GenderAnalytics
                {
                    Gender = g.Key,
                    TotalUnits = g.Sum(x => x.PredictedQuantity),
                    TotalProfit = g.Sum(x => x.PredictedProfit)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ToList();
            report.Sizes = predictions
                .GroupBy(x => x.Size)
                .Select(g => new SizeAnalytics
                {
                    Size = g.Key,
                    TotalUnits = g.Sum(x => x.PredictedQuantity),
                    TotalProfit = g.Sum(x => x.PredictedProfit)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ToList();
            return report;
        }
    }
}
