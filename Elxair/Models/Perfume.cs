namespace Elxair.Models
{
    public class Perfume
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<PerfumeSize> Sizes { get; set; }
    }
}
