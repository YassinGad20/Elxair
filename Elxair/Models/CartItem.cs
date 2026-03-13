namespace Elxair.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; }

        public int PerfumeSizeId { get; set; }

        public PerfumeSize PerfumeSize { get; set; }

        public int Quantity { get; set; }

    }
}
