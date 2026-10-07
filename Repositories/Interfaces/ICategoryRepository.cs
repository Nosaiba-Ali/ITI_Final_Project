using Final_Project.Models;

namespace Final_Project.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesWithProductsAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetByIdWithProductAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
