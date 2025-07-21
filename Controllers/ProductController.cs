using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropBoxMarket.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult All()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .ToList();

        return View(products);
    }

    public IActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);

        return product == null ? NotFound() : View(product);
    }
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        return View(product); 
    }
}
