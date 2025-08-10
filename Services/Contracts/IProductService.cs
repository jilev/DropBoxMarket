using DropBoxMarket.Models;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

public interface IProductService
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(
    int? categoryId, string? searchTerm, int page, int pageSize);
    Task<IEnumerable<Product>> GetRecommendedAsync(IEnumerable<int> excludeIds, int take);

    Task<Product?> GetByIdAsync(int id, bool includeCategory = false);

    Task<Product> CreateAsync(Product product, IFormFile? imageFile);

    Task<bool> UpdateAsync(Product product, IFormFile? imageFile);

    Task<bool> DeleteAsync(int id);

    Task<int> CountCategoriesAsync();

}