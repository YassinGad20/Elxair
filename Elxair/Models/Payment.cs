namespace Elxair.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string CustomerName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // الأربعة دول هم اللي ناقصين
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Governorate { get; set; }
        public string Address { get; set; }
    }
}