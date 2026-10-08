using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModels.Review
{
    public class AddReviewDto
    {
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "التعليق مطلوب")]
        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }
    }
}