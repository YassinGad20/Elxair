namespace Elxair.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int PerfumeSizeId { get; set; }

        public PerfumeSize PerfumeSize { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
