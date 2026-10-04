using Final_Project.ViewModels.Category;
using Final_Project.ViewModels.Product;

namespace Final_Project.Sevices.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListViewModel>> GetAllProductsAsync();
        Task<List<ProductListViewModel>> SearchProductsAsync(string? searchItem, int? categoryId, string? sortOrder);
        Task<List<ProductListViewModel>> GetBySellerIdAsync(string sellerId);
        Task<List<ProductListViewModel>> GetByCategoryIdAsync(int categoryId);
        Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int id);
        Task<ProductCreateViewModel> GetCreateFormAsync();
        Task<ProductEditViewModel?> GetEditFormAsync(int id);
        Task<List<CategoryOptionsViewModel>> GetCategoryOptionsAsync();
        Task<string> GetOwnerIdAsync(int id);
        Task CreateAsync(ProductCreateViewModel viewModel, string sellerId);
        Task UpdateAsync(ProductEditViewModel viewModel, int id);
        Task UpdateStockAsync(int id, int availableQuantity);
        Task DeleteAsync(int Id);
    }
}
