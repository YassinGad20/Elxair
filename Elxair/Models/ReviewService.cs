using Microsoft.EntityFrameworkCore;

namespace Elxair.Models
{
    public class ReviewService
    {
        private readonly ElxairContext _context;

        public ReviewService(ElxairContext context)
        {
            _context = context;
        }
        
        public List<Review> GetPerfumeReview(int PerfumeId)
        {
            return _context.Reviews
                .Where(r => r.PerfumeId == PerfumeId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public double GetAverageRating(int PerfumeId)
        {
            var reviews = _context.Reviews
                .Where(r => r.PerfumeId == PerfumeId);

            if (!reviews.Any())
            {
                return 0;
            }

            return reviews.Average(r => r.Rating);
        }
        public int GetReviewCount(int perfumeId)
        {
            return _context.Reviews.Count(r => r.PerfumeId == perfumeId);
        }

        public bool HasPurchasedPerfume(int UserId,int PerfumeId)
        {
            return _context.OrderItems
                .Any(oi =>
                    oi.Order.UserId == UserId &&
                    oi.Order.Status == "Delivered" &&
                    oi.PerfumeSize.PerfumeId == PerfumeId
                );
        }

        public bool HasReviewed(int UserId ,int PerfumeId)
        {
            return _context.Reviews
                .Any(
                     r => r.UserId == UserId &&
                     r.PerfumeId == PerfumeId
                );
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            review.UpdatedAt = DateTime.Now;

            _context.Reviews.Update(review);
            _context.SaveChanges();
        }
    }
}
