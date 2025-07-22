using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropBoxMarket.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using DropBoxMarket.Models;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult All(int? categoryId)
    {
        var productsQuery = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.SelectedCategory = categoryId;

        var products = productsQuery.ToList();
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
    [HttpPost]
    public IActionResult Create(Product product, IFormFile image)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(product);
        }

        if (image != null && image.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            product.ImageUrl = "/images/" + uniqueFileName;
        }

        _context.Products.Add(product);
        _context.SaveChanges();

        TempData["Message"] = "Product created successfully!";
        return RedirectToAction(nameof(Index));
    }

}
