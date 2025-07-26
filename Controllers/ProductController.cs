using DropBoxMarket.Data;
using DropBoxMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct == null)
            return NotFound();

        existingProduct.Title = product.Title;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.CategoryId = product.CategoryId;

        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            existingProduct.ImageUrl = "/images/" + uniqueFileName;
        }

        _context.SaveChanges();
        TempData["Message"] = "Product updated successfully!";
        return RedirectToAction("All");
    }


    public IActionResult Delete(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
            return NotFound();

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        TempData["Message"] = "Product deleted successfully!";
        return RedirectToAction("All");
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
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

}
