namespace Elxair.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int PerfumeId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
