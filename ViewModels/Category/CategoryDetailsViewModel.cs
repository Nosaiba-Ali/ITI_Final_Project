using Final_Project.ViewModels.Product;
namespace Final_Project.ViewModels.Category
{
    public class CategoryDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<ProductListViewModel> Products { get; set; } = new();
    }
}
