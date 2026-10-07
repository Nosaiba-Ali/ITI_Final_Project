using Final_Project.Models;
using Final_Project.Repositories.Interfaces;
using Final_Project.Sevices.Interfaces;
using Final_Project.ViewModels.Category;
using Final_Project.ViewModels.Product;

namespace Final_Project.Sevices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithProductsAsync();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            }).ToList();

            return categoryViewModels;
        }

        public async Task<CategoryDetailsViewModel?> GetDetailsAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithProductAsync(id);

            if (category == null)
            {
                return null;
            }

            return new CategoryDetailsViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,

                Products = category.Products.Select(p => new ProductListViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    AvailableQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl
                }).ToList()
            };
        }

        public async Task<CategoryEditViewModel> GetForEditAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            var categoryEditViewModel = new CategoryEditViewModel
            {
                Name = category.Name,
                Description = category.Description
            };
            return categoryEditViewModel;
        }

        public async Task<CategoryDeleteViewModel> GetDeleteViewModelAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            var categoryDeleteViewModel = new CategoryDeleteViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
            return categoryDeleteViewModel;
        }

        public async Task CreateAsync(CategoryCreateViewModel viewModel)
        {
            viewModel.Name = viewModel.Name.Trim();

            var exists = await _categoryRepository.ExistsByNameAsync(viewModel.Name);
            if (exists)
            {
                throw new InvalidOperationException("A category with this name already exists.");
            }

            var category = new Category
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task UpdateAsync(int id, CategoryEditViewModel viewModel)
        {
            viewModel.Name = viewModel.Name.Trim();

            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            if (!string.Equals(category.Name, viewModel.Name, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _categoryRepository.ExistsByNameAsync(viewModel.Name);
                if (exists)
                {
                    throw new InvalidOperationException("A category with this name already exists.");
                }
            }

            category.Name = viewModel.Name;
            category.Description = viewModel.Description;

            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            await _categoryRepository.DeleteAsync(category);
        }
    }
}
