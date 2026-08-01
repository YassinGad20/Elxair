using Elxair.Services;
using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class OrderService
    {
        private readonly ElxairContext db;
        private readonly UserService us;
        private readonly IPromotionService promotionService;

        public OrderService(
        ElxairContext db,
        UserService us,
        IPromotionService promotionService)
        {
            this.db = db;
            this.us = us;
            this.promotionService = promotionService;
        }

        public int CreateOrder()
        {
            int userId = us.GetCurrentUserId();

            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.PerfumeSize)
                .ThenInclude(ps => ps.Perfume)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return 0;



            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                int month = DateTime.Now.Month;

                bool soldInSeason = false;

                PerfumeSeason season = item.PerfumeSize.Perfume.Season;

                if (season == PerfumeSeason.Summer)
                {
                    soldInSeason = month >= 4 && month <= 10;
                }
                else if (season == PerfumeSeason.Winter)
                {
                    soldInSeason = month >= 11 || month <= 3;
                }

                decimal finalPrice = promotionService.GetDiscountedPrice(item.PerfumeSize);

                order.Items.Add(new OrderItem
                {
                    PerfumeSizeId = item.PerfumeSizeId,
                    Quantity = item.Quantity,
                    Price = finalPrice,
                    SoldInSeason = soldInSeason
                });

                total += item.Quantity * finalPrice;
            }

            order.TotalPrice = total;

            db.Orders.Add(order);

            // حذف عناصر السلة
            db.CartItems.RemoveRange(cart.Items);

            // حذف السلة نفسها
            db.Carts.Remove(cart);

            db.SaveChanges();
            var user = db.Users.Find(userId);

            if (user != null && user.Role == "Guest")
            {
                user.Role = "Customer";
                db.SaveChanges();
            }

            return order.Id;
        }

        public List<Order> GetUserOrders()
        {
            int userId = us.GetCurrentUserId();

            return db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.PerfumeSize)
                        .ThenInclude(ps => ps.Perfume)
                .ToList();
        }

        public void ClearGuestPayments(int userId)
        {
            var payments = db.Payments
                .Where(p => p.UserId == userId)
                .ToList();

            db.Payments.RemoveRange(payments);
            db.SaveChanges();
        }

        public void ClearGuestOrders(int userId)
        {
            var orders = db.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToList();

            foreach (var order in orders)
            {
                db.OrderItems.RemoveRange(order.Items);
            }

            db.Orders.RemoveRange(orders);

            db.SaveChanges();
        }
    }
}