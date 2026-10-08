using Final_Project.Repositories.Interfaces;
using Final_Project.Sevices.Interfaces;
using Final_Project.ViewModels.Category;
using Final_Project.ViewModels.Product;
using Final_Project.ViewModels.Review;

namespace Final_Project.Sevices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<ProductListViewModel>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsWithDetailsAsync();
            return products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                AvailableQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName
            }).ToList();
        }

        public async Task<List<ProductListViewModel>> SearchProductsAsync(string? searchItem, int? categoryId, string? sortOrder)
        {
            var products = await _productRepository.SearchProductsAsync(searchItem, categoryId, sortOrder);
            return products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                AvailableQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName
            }).ToList();
        }

        public async Task<List<ProductListViewModel>> GetBySellerIdAsync(string sellerId)
        {
            var products = await _productRepository.GetBySellerIdAsync(sellerId);
            return products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                AvailableQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName
            }).ToList();
        }

        public async Task<List<ProductListViewModel>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            return products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                AvailableQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName
            }).ToList();
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int id)
        {
            var p = await _productRepository.GetByIdWithDetailsAsync(id);
            if (p == null)
            {
                return null;
            }
            return new ProductDetailsViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                AvailableQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName,
                ReviewCount = p.Reviews.Count,
                AverageRating = p.Reviews.Any()
                    ? p.Reviews.Average(r => r.Rating)
                    : 0,

                Reviews = p.Reviews.Select(r => new ReviewListItemViewModel
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CustomerName = r.Customer.UserName
                }).ToList()
            };
        }


        public async Task<List<CategoryOptionsViewModel>> GetCategoryOptionsAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithProductsAsync();
            return categories.Select(c => new CategoryOptionsViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<ProductCreateViewModel> GetCreateFormAsync()
        {
            return new ProductCreateViewModel
            {
                AvailableCategories = await GetCategoryOptionsAsync()
            };
        }

        public async Task<ProductEditViewModel?> GetEditFormAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return null;
            }

            var viewModel = new ProductEditViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                AvailableQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                AvailableCategories = await GetCategoryOptionsAsync()
            };
            return viewModel;
        }

        public Task<string> GetOwnerIdAsync(int id)
        {
            var product = _productRepository.GetProductByIdAsync(id);
            return product.ContinueWith(t => t.Result?.SellerId ?? string.Empty);
        }

        public async Task CreateAsync(ProductCreateViewModel viewModel, string sellerId)
        {
            viewModel.Name = viewModel.Name?.Trim();
            var category = await _categoryRepository.GetCategoryByIdAsync(viewModel.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid category ID.");
            }
            if (await _productRepository.ExistsByNameforSeller(viewModel.Name, sellerId))
            {
                throw new ArgumentException("Product name already exists for this seller.");
            }

            var product = new Models.Product
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                StockQuantity = viewModel.AvailableQuantity,
                ImageUrl = viewModel.ImageUrl,
                CategoryId = viewModel.CategoryId,
                SellerId = sellerId
            };
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(ProductEditViewModel viewModel, int id)
        {
            viewModel.Name = viewModel.Name?.Trim();
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new ArgumentException("Product not found.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(viewModel.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid category ID.");
            }

            if (await _productRepository.ExistsByNameforSeller(viewModel.Name, existingProduct.SellerId, id))
            {
                throw new ArgumentException("Product name already exists for this seller.");
            }

            existingProduct.Name = viewModel.Name;
            existingProduct.Description = viewModel.Description;
            existingProduct.Price = viewModel.Price;
            existingProduct.StockQuantity = viewModel.AvailableQuantity;
            existingProduct.ImageUrl = viewModel.ImageUrl;
            existingProduct.CategoryId = viewModel.CategoryId;

            await _productRepository.UpdateAsync(existingProduct);

        }

        public async Task UpdateStockAsync(int id, int availableQuantity)
        {
            if (availableQuantity < 0)
            {
                throw new ArgumentException("Available quantity cannot be negative.");
            }

            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }

            product.StockQuantity = availableQuantity;
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(int Id)
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(Id);
            if (product == null)
            {
                throw new ArgumentException("Product not found.");
            }

            _productRepository.Delete(product);
        }
    }
}
