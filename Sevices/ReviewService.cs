using Final_Project.Data; 
using Final_Project.Models;
using Final_Project.ViewModels.Review;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddReviewAsync(AddReviewDto dto, string customerId)
        {
            var review = new Review
            {
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(review);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<ReviewListItemViewModel>> GetProductReviewsAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewListItemViewModel
                {
                    Id = r.Id,
                    CustomerName = r.Customer.UserName ?? "Customer",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any()) return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}