using Final_Project.Data;
using Final_Project.Models;
using Final_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Product>> GetAllProductsWithDetailsAsync()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public Task<List<Product>> SearchProductsAsync(string? searchItem, int? categoryId, string? sortOrder)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchItem.ToLower()) || p.Description.ToLower().Contains(searchItem.ToLower()));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                switch (sortOrder)
                {
                    case "name":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                }
            }

            return query.ToListAsync();
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            return _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<Product?> GetByIdWithDetailsAsync(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Product>> GetBySellerIdAsync(string sellerId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();
        }

        public Task<List<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public Task<bool> ExistsByNameforSeller(string name, string sellerId, int? excludeId = null)
        {
            return _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.SellerId == sellerId && (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
