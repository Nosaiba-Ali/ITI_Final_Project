using Final_Project.Data;
using Final_Project.Models;
using Final_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Category>> GetAllCategoriesWithProductsAsync()
        {
            return _context.Categories.Include(c => c.Products).ToListAsync();
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            return _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Category> GetByIdWithProductAsync(int id)
        {
            return _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
