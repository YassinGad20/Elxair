using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class PaymentService
    {
        ElxairContext db = new ElxairContext();

        public void CreatePayment(int orderId, int userId, string customerName,
                                  decimal amount, string fullName,
                                  string phone, string governorate, string address)
        {
            var exists = db.Payments.Any(p => p.OrderId == orderId);
            if (exists) return;

            var payment = new Payment
            {
                OrderId = orderId,
                UserId = userId,
                CustomerName = customerName,
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = "Pending",
                FullName = fullName,
                Phone = phone,
                Governorate = governorate,
                Address = address
            };

            db.Payments.Add(payment);
            db.SaveChanges();
        }

        // جيب الـ Payment بتاع Order معين
        public Payment? GetByOrderId(int orderId)
        {
            return db.Payments
                .Include(p => p.Order)
                .Include(p => p.User)
                .FirstOrDefault(p => p.OrderId == orderId);
        }

        // جيب كل الـ Payments (للـ Admin)
        public List<Payment> GetAllPayments()
        {
            return db.Payments
                .Include(p => p.Order)
                .Include(p => p.User)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        // تحديث حالة الدفع (للـ Admin — Cash بيتأكد بعدين)
        public void UpdateStatus(int paymentId, string status)
        {
            var payment = db.Payments.Find(paymentId);
            if (payment != null)
            {
                payment.Status = status;
                db.SaveChanges();
            }
        }
    }
}