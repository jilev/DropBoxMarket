using DropBoxMarket.Data;
using DropBoxMarket.Models;
using DropBoxMarket.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DropBoxMarket.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _db;
    public CategoryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<Category>> GetAllAsync()
            => _db.Categories
                  .OrderBy(c => c.Name)
                  .ToListAsync();

        public Task<Category?> GetByIdAsync(int id)
            => _db.Categories
                  .FirstOrDefaultAsync(c => c.Id == id);

        public Task<int> CountAsync()
            => _db.Categories.CountAsync();

        public Task<bool> ExistsAsync(int id)
            => _db.Categories.AnyAsync(c => c.Id == id);

        public Task<bool> NameExistsAsync(string name, int? excludeId = null)
            => _db.Categories.AnyAsync(c =>
                   c.Name == name && (!excludeId.HasValue || c.Id != excludeId.Value));

        public async Task<Category> CreateAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _db.Categories.FindAsync(category.Id);
            if (existing is null) return false;

            existing.Name = category.Name;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat is null) return false;

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}