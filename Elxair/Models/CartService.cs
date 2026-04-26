using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class CartService
    {
        ElxairContext db = new ElxairContext();

        public void AddToCart(int userId, int perfumeSizeId, int quantity)
        {
            var perfumeSize = db.PerfumeSizes.Find(perfumeSizeId);

            if (perfumeSize == null || perfumeSize.Stock < quantity)
                throw new Exception("Sorry, out of stock!");

            var cart = db.Carts.Include(c => c.Items)
                               .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var existing = cart.Items.FirstOrDefault(i => i.PerfumeSizeId == perfumeSizeId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    PerfumeSizeId = perfumeSizeId,
                    Quantity = quantity
                });
            }

            perfumeSize.Stock -= quantity;
            db.SaveChanges();
        }

        public void RemoveFromCart(int itemId)
        {
            var item = db.CartItems.Find(itemId);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }
        }

        public List<CartItem> GetUserCart(int userId)
        {
            return db.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .Include(i => i.PerfumeSize)
                .ThenInclude(p => p.Perfume)
                .ToList();
        }
    }
}