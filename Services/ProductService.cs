using DropBoxMarket.Data;
using DropBoxMarket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

public ProductService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(
        int? categoryId, string? searchTerm, int page, int pageSize)
    {
        var q = _db.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(p => p.Title.Contains(searchTerm) || p.Description.Contains(searchTerm));

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Product>> GetRecommendedAsync(IEnumerable<int> excludeIds, int take)
        => await _db.Products
            .Include(p => p.Category)
            .Where(p => !excludeIds.Contains(p.Id))
            .OrderBy(_ => Guid.NewGuid())
            .Take(take)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int id, bool includeCategory = false)
        => includeCategory
            ? await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id)
            : await _db.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product, IFormFile? imageFile)
    {
        if (imageFile is not null && imageFile.Length > 0)
            product.ImageUrl = await SaveImageAsync(imageFile);

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product, IFormFile? imageFile)
    {
        var existing = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existing is null) return false;

        existing.Title = product.Title;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.CategoryId = product.CategoryId;

        if (imageFile is not null && imageFile.Length > 0)
            existing.ImageUrl = await SaveImageAsync(imageFile);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var path = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (File.Exists(path)) File.Delete(path);
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountCategoriesAsync() => _db.Categories.CountAsync();

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var imagesDir = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(imagesDir);

        var name = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(imagesDir, name);
        using (var fs = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(fs);

        return $"/images/{name}";
    }


}