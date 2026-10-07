using Final_Project.ViewModels.Category;

namespace Final_Project.Sevices.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllCategoriesAsync();
        Task<CategoryDetailsViewModel?> GetDetailsAsync(int id);
        Task<CategoryEditViewModel> GetForEditAsync(int id);
        Task<CategoryDeleteViewModel> GetDeleteViewModelAsync(int id);
        Task CreateAsync(CategoryCreateViewModel viewModel);
        Task UpdateAsync(int id, CategoryEditViewModel viewModel);
        Task DeleteAsync(int id);
    }
}
