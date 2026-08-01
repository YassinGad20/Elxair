using Elxair.Services;
using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ProductService
    {
        private readonly ElxairContext db;
        private readonly IPromotionService promotionService;

        public ProductService(
        ElxairContext db,
        IPromotionService promotionService)
        {
            this.db = db;
            this.promotionService = promotionService;
        }
        public List<Perfume> SearchAndFilter(string? search, string? gender, int? categoryId)
        {
            var query = db.Perfumes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Brand.Contains(search) ||
                    p.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(gender))
            {
                query = query.Where(p => p.Gender == gender);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return query.ToList();
        }
        public List<Perfume> GetAllPerfumes()
        {
            var perfumes = db.Perfumes
            .Include(p => p.Category)
            .Include(p => p.Sizes)
            .ToList();

            foreach (var perfume in perfumes)
            {
                ApplyPromotionData(perfume);
            }

            return perfumes;
        }

        public Perfume? GetPerfume(int id)
        {
            var perfume = db.Perfumes
            .Include(p => p.Category)
            .Include(p => p.Sizes)
            .FirstOrDefault(p => p.Id == id);

            if (perfume != null)
            {
                ApplyPromotionData(perfume);
            }

            return perfume;
        }

        public void AddPerfume(Perfume perfume)
        {
            db.Perfumes.Add(perfume);
            db.SaveChanges();
        }

        public void AddPerfumeSize(int perfumeId, string sizeName, decimal price, int stock)
        {
            var perfumeSize = new PerfumeSize
            {
                PerfumeId = perfumeId,
                Size = sizeName,
                Price = price,
                Stock = stock
            };

            db.PerfumeSizes.Add(perfumeSize);
            db.SaveChanges();
        }

        public void DeletePerfume(int id)
        {
            var perfume = db.Perfumes
                .Include(p => p.Sizes)
                .FirstOrDefault(p => p.Id == id);

            if (perfume != null)
            {
                if (perfume.Sizes != null && perfume.Sizes.Any())
                {
                    db.PerfumeSizes.RemoveRange(perfume.Sizes);
                }

                db.Perfumes.Remove(perfume);
                db.SaveChanges();
            }
        }
        public List<Perfume> GetRecommendations(Perfume current)
        {
            var perfumes = db.Perfumes
            .Include(p => p.Category)
            .Include(p => p.Sizes)
            .Where(p =>
                p.Id != current.Id &&
                p.Gender == current.Gender &&
                p.CategoryId == current.CategoryId)
            .Take(4)
            .ToList();

            foreach (var perfume in perfumes)
            {
                ApplyPromotionData(perfume);
            }

            return perfumes;
        }

        private void ApplyPromotionData(Perfume perfume)
        {
            if (perfume.Sizes == null || !perfume.Sizes.Any())
                return;

            var firstSize = perfume.Sizes.OrderBy(s => s.Price).First();

            perfume.DisplayPrice = firstSize.Price;
            perfume.HasPromotion = false;

            foreach (var size in perfume.Sizes)
            {
                var promotion = promotionService.GetActivePromotion(size);

                if (promotion != null)
                {
                    var discountedPrice = promotionService.GetDiscountedPrice(size);

                    if (!perfume.HasPromotion || discountedPrice < perfume.DisplayPrice)
                    {
                        perfume.HasPromotion = true;
                        perfume.OldPrice = size.Price;
                        perfume.DisplayPrice = discountedPrice;
                        perfume.DiscountPercentage =
                            promotionService.GetDiscountPercentage(size);
                    }
                }
            }
        }


    }
}