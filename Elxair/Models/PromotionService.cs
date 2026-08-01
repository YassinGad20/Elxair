using Elxair.Models;
using Microsoft.EntityFrameworkCore;

namespace Elxair.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly ElxairContext _context;

        public PromotionService(ElxairContext context)
        {
            _context = context;
        }

        public Promotion? GetActivePromotion(PerfumeSize perfumeSize)
        {
            return _context.PromotionPerfumeSizes
                .Include(x => x.Promotion)
                .Where(x =>
                    x.PerfumeSizeId == perfumeSize.Id &&
                    x.Promotion.IsActive &&
                    x.Promotion.StartDate <= DateTime.Now &&
                    x.Promotion.EndDate >= DateTime.Now)
                .Select(x => x.Promotion)
                .FirstOrDefault();
        }

        public bool HasPromotion(PerfumeSize perfumeSize)
        {
            return GetActivePromotion(perfumeSize) != null;
        }

        public decimal GetDiscountAmount(PerfumeSize perfumeSize)
        {
            var promotion = GetActivePromotion(perfumeSize);

            if (promotion == null)
                return 0;

            if (promotion.PromotionType == PromotionType.Percentage)
            {
                return perfumeSize.Price * promotion.Value / 100;
            }

            return promotion.Value;
        }

        public decimal GetDiscountedPrice(PerfumeSize perfumeSize)
        {
            var discount = GetDiscountAmount(perfumeSize);

            var finalPrice = perfumeSize.Price - discount;

            return finalPrice < 0 ? 0 : finalPrice;
        }

        public decimal GetDiscountPercentage(PerfumeSize perfumeSize)
        {
            var promotion = GetActivePromotion(perfumeSize);

            if (promotion == null)
                return 0;

            if (promotion.PromotionType == PromotionType.Percentage)
                return promotion.Value;

            return Math.Round((promotion.Value / perfumeSize.Price) * 100, 2);
        }
    }
}