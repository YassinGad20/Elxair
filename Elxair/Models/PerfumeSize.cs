namespace Elxair.Models
{
    public class PerfumeSize
    {
        public int Id { get; set; }

        public int PerfumeId { get; set; }
        public Perfume Perfume { get; set; }

        public string Size { get; set; }   // 50ml - 100ml - 200ml

        public decimal Price { get; set; }

        public int Stock { get; set; }

    }
}
