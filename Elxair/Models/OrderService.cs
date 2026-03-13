using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{

    public class OrderService
    {
        ElxairContext db = new ElxairContext();


        public void CreateOrder(int userId)
        {
            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.PerfumeSize)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return;

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
                var orderItem = new OrderItem
                {
                    PerfumeSizeId = item.PerfumeSizeId,
                    Quantity = item.Quantity,
                    Price = item.PerfumeSize.Price
                };

                total += item.Quantity * item.PerfumeSize.Price;

                order.Items.Add(orderItem);
            }

            order.TotalPrice = total;

            db.Orders.Add(order);

            db.CartItems.RemoveRange(cart.Items);

            db.SaveChanges();
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
