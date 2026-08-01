using Elxair.Models;

namespace Elxair.Services
{
    public interface IPromotionService
    {
        Promotion? GetActivePromotion(PerfumeSize perfumeSize);

        bool HasPromotion(PerfumeSize perfumeSize);

        decimal GetDiscountAmount(PerfumeSize perfumeSize);

        decimal GetDiscountedPrice(PerfumeSize perfumeSize);

        decimal GetDiscountPercentage(PerfumeSize perfumeSize);
    }
}