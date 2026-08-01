using System.Net.Http.Json;
using System.Text.Json;
using Elxair.Models;
using Microsoft.EntityFrameworkCore;


namespace Elxair.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;
        private readonly ElxairContext db;

        public AiService(HttpClient httpClient , ElxairContext db)
        {
            this.db = db;
            _httpClient = httpClient;
            
        }

        public List<PerfumeSize> GetAllPerfumeSizes()
        {
            return db.PerfumeSizes
                .Include(ps => ps.Perfume)
                .ThenInclude(p => p.Category)
                .ToList();
        }



        private string GetMonthName(int month)
        {
            return new DateTime(2026, month, 1)
                .ToString("MMMM");
        }

        private decimal GetDiscountedPrice(PerfumeSize perfumeSize)
        {
            var promotion = db.PromotionPerfumeSizes
                .Include(x => x.Promotion)
                .Where(x =>
                    x.PerfumeSizeId == perfumeSize.Id &&
                    x.Promotion.IsActive &&
                    x.Promotion.StartDate <= DateTime.Now &&
                    x.Promotion.EndDate >= DateTime.Now)
                .Select(x => x.Promotion)
                .FirstOrDefault();

            if (promotion == null)
                return perfumeSize.Price;

            if (promotion.PromotionType == PromotionType.Percentage)
            {
                return perfumeSize.Price * (1 - promotion.Value / 100);
            }

            return perfumeSize.Price - promotion.Value;
        }


        private int GetSoldInSeason(PerfumeSeason season, int month)
        {
            switch (season)
            {
                case PerfumeSeason.AllSeasons:
                    return 1;

                case PerfumeSeason.Summer:
                    return (month >= 4 && month <= 10) ? 1 : 0;

                case PerfumeSeason.Winter:
                    return (month == 11 ||
                            month == 12 ||
                            month == 1 ||
                            month == 2 ||
                            month == 3)
                        ? 1
                        : 0;

                default:
                    return 0;
            }
        }
        private int ExtractVolumeMl(string sizeText)
        {
            var digits = new string((sizeText ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            return int.TryParse(digits, out var value)
                ? value
                : 0;
        }

        private string GetPriceCategory(decimal price)
        {
            if (price <= 150)
                return "Cheap";

            if (price <= 400)
                return "Mid-Range";

            return "Premium";
        }

        public PredictionReportRequest BuildPredictionReportRequest(
                int month,
                int year
            )
        {
            var perfumeSizes = db.PerfumeSizes
                .Include(x => x.Perfume)
                .ThenInclude(x => x.Category)
                .ToList();

            var items = new List<PredictionItem>();

            foreach (var perfumeSize in perfumeSizes)
            {
                items.Add(
                    BuildPredictionItem(
                            perfumeSize,
                            month,
                            year
                        
                       )
                    );
            }
            return  new PredictionReportRequest
            {
                PredictionMonth = month,
                PredictionYear = year,
                Items = items
            }; ;
        }

        public PredictionItem BuildPredictionItem(
            PerfumeSize perfumeSize,
            int month,
            int year)
        {
            int numberOfTransactions = db.OrderItems
            .Where(oi =>
                oi.PerfumeSizeId == perfumeSize.Id &&
                oi.Order.OrderDate.Month == month &&
                oi.Order.OrderDate.Year == year)
            .Select(oi => oi.OrderId)
            .Distinct()
            .Count();






            int bottleVolume = ExtractVolumeMl(perfumeSize.Size);

            double avgDiscount = db.PromotionPerfumeSizes
                .Include(x => x.Promotion)
                .Where(x =>
                    x.PerfumeSizeId == perfumeSize.Id &&
                    x.Promotion.IsActive &&
                    x.Promotion.StartDate <= DateTime.Now &&
                    x.Promotion.EndDate >= DateTime.Now)
                .Select(x => (double?)x.Promotion.Value)
                .FirstOrDefault() ?? 0;

            string priceCategory =
                GetPriceCategory(perfumeSize.Price);

            decimal avgMarginProfit =
                perfumeSize.Price - perfumeSize.CostPerBottle;

            int SoldInSeason = GetSoldInSeason(
                      perfumeSize.Perfume.Season,
                      month
            );

           

            return new PredictionItem
            {
                PerfumeId = perfumeSize.Perfume.Id,

                PerfumeSizeId = perfumeSize.Id,

                PerfumeName = perfumeSize.Perfume.Name,

                Month = GetMonthName(month),

                BottleSize = perfumeSize.Size,

                BottleVolume = bottleVolume,

                Category = perfumeSize.Perfume.Category.Name,

                Gender = perfumeSize.Perfume.Gender,

                Season = perfumeSize.Perfume.Season.ToString(),

                SoldInSeason = SoldInSeason,

                PriceCategory = priceCategory,

                AvgMarginProfit = (double)avgMarginProfit,

                AvgDiscount = avgDiscount,

                NumberOfTransactions = numberOfTransactions
            };
        }


        public async Task<PredictionReport> GetPredictionReportAsync(
          PredictionReportRequest request)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://127.0.0.1:8080/predict/report",
                request,
                options);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            var report = await response.Content.ReadFromJsonAsync<PredictionReport>();

            return report!;
        }

        public async Task<List<DashboardPredictionResult>> PredictAllPerfumes(
     int month,
     int year)
        {
            var perfumeSizes = GetAllPerfumeSizes();

            var request = BuildPredictionReportRequest(month, year);

            var report = await GetPredictionReportAsync(request);

            var dashboardResults = new List<DashboardPredictionResult>();

            var perfumeSizeLookup = perfumeSizes.ToDictionary(
                    x => x.Id
                );


            foreach (var prediction in report.Predictions)
            {
                var perfumeSize = perfumeSizeLookup[prediction.PerfumeSizeId];

                dashboardResults.Add(
                    new DashboardPredictionResult
                    {
                        PerfumeId = prediction.PerfumeId,

                        PerfumeSizeId = prediction.PerfumeSizeId,

                        PerfumeName = prediction.PerfumeName,

                        CategoryName = perfumeSize.Perfume.Category.Name,

                        Gender = perfumeSize.Perfume.Gender,

                        Size = perfumeSize.Size,

                        PredictedQuantity = prediction.PredictedQuantity,

                        PredictedProfit =
                            prediction.PredictedQuantity *
                            (perfumeSize.Price - perfumeSize.CostPerBottle)
                });
                
            }
            return dashboardResults;
        }
        
    }
}