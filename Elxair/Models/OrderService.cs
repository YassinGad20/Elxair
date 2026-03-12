using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{

    public class OrderService
    {
        ElxairContext db = new ElxairContext();

        public List<Order> GetAllOrders()
        {
            return db.Orders.ToList();
        }
        public void CreateOrder(int userId)
        {
            var cart = db.Carts
                         .Include(c => c.Items)
                         .ThenInclude(ci => ci.Perfume)
                         .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
                throw new Exception("Cart is empty.");


            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                Items = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            
            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    PerfumeId = item.PerfumeId,
                    Quantity = item.Quantity,
                    Price = item.Perfume.Price * item.Quantity
                };

                totalPrice += orderItem.Price;

                order.Items.Add(orderItem);
            }

            order.TotalPrice = totalPrice;

            
            db.Orders.Add(order);

            
            db.CartItems.RemoveRange(cart.Items);

            db.SaveChanges();
        }

        public List<Order> GetUserOrders(int userId)
        {
          return db.Orders
         .Where(o => o.UserId == userId)
         .ToList();
        }

    }
}
