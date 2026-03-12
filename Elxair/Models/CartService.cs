namespace Elxair.Models
{

    public class CartService
    {
        ElxairContext db = new ElxairContext();
        public void AddToCart(int userId, int perfumeId, int quantity)
        {
            
            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);

            
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

           
            var cartItem = db.CartItems
                             .FirstOrDefault(ci => ci.CartId == cart.Id && ci.PerfumeId == perfumeId);

            if (cartItem != null)
            {
                
                cartItem.Quantity += quantity;
                db.CartItems.Update(cartItem);
            }
            else
            {
                
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    PerfumeId = perfumeId,
                    Quantity = quantity
                };
                db.CartItems.Add(cartItem);
            }

            db.SaveChanges();

        }

        public void RemoveFromCart(int itemId)
        {
            var cartItem = db.CartItems.FirstOrDefault(ci => ci.Id == itemId);

            if (cartItem != null)
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
            }

        }

        public List<CartItem> GetUserCart(int userId)
        {

            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return new List<CartItem>();

            return db.CartItems
                     .Where(ci => ci.CartId == cart.Id)
                     .ToList();

        }

    }
}
