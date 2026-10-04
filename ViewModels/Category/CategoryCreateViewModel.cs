using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModels.Category
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters long.")]
        [MaxLength(22, ErrorMessage = "Category name cannot exceed 22 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Category description is required.")]
        public string? Description { get; set; }
    }
}
