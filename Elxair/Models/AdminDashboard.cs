namespace Elxair.Models
{
    public class AdminDashboard
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<string> CategoryNames { get; set; }
        public List<int> CategorySales { get; set; }

        public List<PerfumeSalesDto> PerfumeComparison { get; set; }

        public List<string> Months { get; set; }
        public List<decimal> MonthlyProfits { get; set; }
    }

    public class PerfumeSalesDto
    {
        public string Name { get; set; }
        public int SalesCount { get; set; }
        public decimal Revenue { get; set; }
    }
}