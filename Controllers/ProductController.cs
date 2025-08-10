using DropBoxMarket.Data;
using DropBoxMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize] 
public class ProductController : Controller
{
    private readonly IProductService _products;
    private readonly ApplicationDbContext _db; 

public ProductController(IProductService products, ApplicationDbContext db)
    {
        _products = products;
        _db = db;
    }

    private async Task SetCategoryCountAsync()
        => ViewBag.CategoryCount = await _products.CountCategoriesAsync();

    [AllowAnonymous]
    public async Task<IActionResult> All(int? categoryId, string? searchTerm, int page = 1)
    {
        await SetCategoryCountAsync();

        ViewData["Breadcrumbs"] = new List<(string, string?)>
    {
        ("Home", Url.Action("Index", "Home")),
        ("Products", null)
    };

        const int pageSize = 6;
        var (items, total) = await _products.GetAllAsync(categoryId, searchTerm, page, pageSize);

        ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name");
        ViewBag.SelectedCategory = categoryId;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.RecommendedProducts = await _products.GetRecommendedAsync(items.Select(x => x.Id), 4);

        return View(items);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        await SetCategoryCountAsync();

        var product = await _products.GetByIdAsync(id, includeCategory: true);
        if (product is null) return NotFound();

        ViewData["Breadcrumbs"] = new List<(string, string?)>
    {
        ("Home", Url.Action("Index", "Home")),
        ("Products", Url.Action("All", "Product")),
        (product.Title, null)
    };

        return View(product);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id)
    {
        await SetCategoryCountAsync();

        var product = await _products.GetByIdAsync(id);
        if (product is null) return NotFound();

        ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
        ViewData["Breadcrumbs"] = new List<(string, string?)>
    {
        ("Home", Url.Action("Index", "Home")),
        ("Products", Url.Action("All", "Product")),
        ("Edit", null)
    };
        return View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
    {
        await SetCategoryCountAsync();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        var ok = await _products.UpdateAsync(product, imageFile);
        if (!ok) return NotFound();

        TempData["Message"] = "Product updated successfully!";
        return RedirectToAction(nameof(All));
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create()
    {
        await SetCategoryCountAsync();
        ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name");
        return View(new Product());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        await SetCategoryCountAsync();

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        await _products.CreateAsync(product, imageFile);
        TempData["Message"] = "Product created!";
        return RedirectToAction(nameof(All));
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        await SetCategoryCountAsync();

        var product = await _products.GetByIdAsync(id, includeCategory: true);
        if (product is null) return NotFound();

        ViewData["Breadcrumbs"] = new List<(string, string?)>
    {
        ("Home", Url.Action("Index", "Home")),
        ("Products", Url.Action("All", "Product")),
        ("Delete", null)
    };
        return View(product);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await SetCategoryCountAsync();

        var ok = await _products.DeleteAsync(id);
        if (!ok) return NotFound();

        TempData["Message"] = "Product deleted successfully!";
        return RedirectToAction(nameof(All));
    }


}