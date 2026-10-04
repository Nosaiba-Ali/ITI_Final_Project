using Final_Project.ViewModels.Category;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModels.Product
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [StringLength(1000, ErrorMessage = "Product description cannot exceed 1000 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Product price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Available quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available quantity must be a positive integer.")]
        public int AvailableQuantity { get; set; }
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        public List<CategoryOptionsViewModel> AvailableCategories { get; set; } = new List<CategoryOptionsViewModel>();
    }
}
