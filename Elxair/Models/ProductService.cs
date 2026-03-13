using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ProductService
    {
        ElxairContext db = new ElxairContext();

        public List<Perfume> GetAllPerfumes()
        {
            return db.Perfumes
                .Include(p => p.Sizes)
                .ToList();
        }

        public Perfume GetPerfume(int id)
        {
            return db.Perfumes
                .Include(p => p.Sizes)
                .FirstOrDefault(p => p.Id == id);
        }

        public void AddPerfume(Perfume perfume)
        {
            db.Perfumes.Add(perfume);
            db.SaveChanges();
        }

        public void AddPerfumeSize(int perfumeId, string size, decimal price, int stock)
        {
            var perfumeSize = new PerfumeSize
            {
                PerfumeId = perfumeId,
                Size = size,
                Price = price,
                Stock = stock
            };

            db.PerfumeSizes.Add(perfumeSize);
            db.SaveChanges();
        }

        public void DeletePerfume(int id)
        {
            var perfume = db.Perfumes.Find(id);

            db.Perfumes.Remove(perfume);
            db.SaveChanges();
        }

    }
}
