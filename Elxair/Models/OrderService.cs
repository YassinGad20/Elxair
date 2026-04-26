using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{

    public class OrderService
    {
        ElxairContext db = new ElxairContext();


        // غير الـ return type من void لـ int
        public int CreateOrder(int userId)
        {
            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.PerfumeSize)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any()) return 0;

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
                order.Items.Add(new OrderItem
                {
                    PerfumeSizeId = item.PerfumeSizeId,
                    Quantity = item.Quantity,
                    Price = item.PerfumeSize.Price
                });
                total += item.Quantity * item.PerfumeSize.Price;
            }

            order.TotalPrice = total;
            db.Orders.Add(order);
            db.CartItems.RemoveRange(cart.Items);
            db.SaveChanges();

            return order.Id; // ← بيرجع الـ ID عشان نعمل Payment بيه
        }

        public List<Order> GetUserOrders(int userId)
        {
            return db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.PerfumeSize)
                .ThenInclude(p => p.Perfume)
                .ToList();
        }

    }
}
