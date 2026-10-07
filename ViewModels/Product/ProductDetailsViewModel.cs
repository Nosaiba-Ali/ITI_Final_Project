using Final_Project.ViewModels.Review;

namespace Final_Project.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public string ImageUrl { get; set; }

        public string CategoryName { get; set; }
        public string SellerName { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewListItemViewModel> Reviews { get; set; } = new List<ReviewListItemViewModel>();

        public bool IsInWishlist { get; set; }
        public bool HasReviewed { get; set; }
        public bool CanReview { get; set; }
    }
}
