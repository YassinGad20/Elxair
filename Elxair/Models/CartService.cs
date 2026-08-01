using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class CartService
    {
        private readonly ElxairContext db;
        private readonly UserService us;

        public CartService(ElxairContext db, UserService us)
        {
            this.db = db;
            this.us = us;
        }

        public void AddToCart(int perfumeSizeId, int quantity)
        {
            int userId = us.GetCurrentUserId();

            var perfumeSize = db.PerfumeSizes.Find(perfumeSizeId);

            if (perfumeSize == null)
                throw new Exception("Perfume size not found.");

            if (perfumeSize.Stock < quantity)
                throw new Exception("Sorry, out of stock!");

            var cart = db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };

                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var existingItem = cart.Items
                .FirstOrDefault(i => i.PerfumeSizeId == perfumeSizeId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
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

            db.SaveChanges();
        }

        public List<CartItem> GetUserCart()
        {
            int userId = us.GetCurrentUserId();

            return db.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .Include(i => i.PerfumeSize)
                .ThenInclude(ps => ps.Perfume)
                .ToList();
        }

        public void RemoveFromCart(int itemId)
        {
            int userId = us.GetCurrentUserId();

            var item = db.CartItems
                .Include(i => i.Cart)
                .FirstOrDefault(i => i.Id == itemId && i.Cart.UserId == userId);

            if (item == null)
                throw new Exception("Cart item not found.");

            db.CartItems.Remove(item);
            db.SaveChanges();
        }

        public void ClearCart(int userId)
        {
            var cart = db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return;

            db.CartItems.RemoveRange(cart.Items);
            db.Carts.Remove(cart);

            db.SaveChanges();
        }
    }
}