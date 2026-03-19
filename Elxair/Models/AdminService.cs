using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class AdminService
    {
        ElxairContext db = new ElxairContext();

        public List<Perfume> GetAllPerfumes()
        {
            return db.Perfumes
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .ToList();
        }

        public List<Order> GetAllOrders()
        {
            return db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.PerfumeSize)
                .ThenInclude(ps => ps.Perfume)
                .ToList();
        }

        public void AddPerfume(Perfume perfume)
        {
            db.Perfumes.Add(perfume);
            db.SaveChanges();
        }

        public void UpdatePerfume(Perfume perfume)
        {
            db.Perfumes.Update(perfume);
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

        public void UpdatePerfumeSize(PerfumeSize size)
        {
            db.PerfumeSizes.Update(size);
            db.SaveChanges();
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
        public List<Category> GetAllCategories()
        {
            return db.Categories.ToList();
        }
    }
}