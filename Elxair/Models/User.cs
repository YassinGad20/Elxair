namespace Elxair.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<Order>? Orders { get; set; }
        public Cart? Cart { get; set; }
        public List<Payment>? Payments { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}