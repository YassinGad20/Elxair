using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ProductService
    {
        ElxairContext db = new ElxairContext();

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
    }
}