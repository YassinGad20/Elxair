using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ProductService
    {
        private readonly ElxairContext db;

        public ProductService(ElxairContext db)
        {
            this.db = db;
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
            return db.Perfumes
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .ToList();
        }

        public Perfume? GetPerfume(int id)
        {
            return db.Perfumes
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .FirstOrDefault(p => p.Id == id);
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
            return db.Perfumes
                .Where(p =>
                    p.Id != current.Id &&
                    p.Gender == current.Gender &&
                    p.CategoryId == current.CategoryId)
                .Take(4)
                .ToList();
        }
    }
}