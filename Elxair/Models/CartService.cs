using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{

    public class CartService
    {
        ElxairContext db = new ElxairContext();


        public void AddToCart(int userId, int perfumeSizeId, int quantity)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var item = db.CartItems
                .FirstOrDefault(i => i.CartId == cart.Id && i.PerfumeSizeId == perfumeSizeId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                item = new CartItem
                {
                    CartId = cart.Id,
                    PerfumeSizeId = perfumeSizeId,
                    Quantity = quantity
                };

                db.CartItems.Add(item);
            }

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
