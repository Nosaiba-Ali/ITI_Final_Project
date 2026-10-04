using Final_Project.Models;

namespace Final_Project.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsWithDetailsAsync();
        Task<List<Product>> SearchProductsAsync(string? searchItem, int? categoryId, string? sortOrder);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetByIdWithDetailsAsync(int id);
        Task<List<Product>> GetBySellerIdAsync(string sellerId);
        Task<List<Product>> GetByCategoryIdAsync(int categoryId);
        Task<bool> ExistsByNameforSeller(string name, string sellerId, int? excludeId = null);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        void Delete(Product product);
    }
}
