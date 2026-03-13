using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class AdminService
    {

        ElxairContext db = new ElxairContext();

        public List<Perfume> GetAllPerfumes()
        {
            return db.Perfumes
                .Include(p => p.Name).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.PerfumeSize)
                .ThenInclude(p => p.Perfume)
                .ToList();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);

            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
            }
        }

        public void AddPerfume(Perfume perfume)
        {
            db.Perfumes.Add(perfume);
            db.SaveChanges();
        }

        public void AddPerfumeSize(int perfumeId, string size, decimal price, int stock)
        {
            PerfumeSize perfumeSize = new PerfumeSize
            {
                PerfumeId = perfumeId,
                Size = size,
                Price = price,
                Stock = stock
            };

            db.PerfumeSizes.Add(perfumeSize);
            db.SaveChanges();
        }

        public void UpdatePerfume(Perfume perfume)
        {
            var existing = db.Perfumes.Find(perfume.Id);

            if (existing != null)
            {
                existing.Name = perfume.Name;
                existing.Brand = perfume.Brand;
                existing.Description = perfume.Description;
                existing.ImageUrl = perfume.ImageUrl;
                existing.CategoryId = perfume.CategoryId;

                db.SaveChanges();
            }
        }

        public void UpdatePerfumeSize(PerfumeSize size)
        {
            var existing = db.PerfumeSizes.Find(size.Id);

            if (existing != null)
            {
                existing.Size = size.Size;
                existing.Price = size.Price;
                existing.Stock = size.Stock;

                db.SaveChanges();
            }
        }

        public void DeletePerfume(int perfumeId)
        {
            var sizes = db.PerfumeSizes
                .Where(s => s.PerfumeId == perfumeId)
                .ToList();

            db.PerfumeSizes.RemoveRange(sizes);

            var perfume = db.Perfumes.Find(perfumeId);

            if (perfume != null)
            {
                db.Perfumes.Remove(perfume);
            }

            db.SaveChanges();
        }


    }
}

