using Final_Project.ViewModels.Review;

namespace Final_Project.Services
{
    public interface IReviewService
    {
        Task<bool> AddReviewAsync(AddReviewDto dto, string customerId);
        Task<IEnumerable<ReviewListItemViewModel>> GetProductReviewsAsync(int productId);
        Task<double> GetAverageRatingAsync(int productId);
    }
}