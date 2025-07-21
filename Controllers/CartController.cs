using Microsoft.AspNetCore.Mvc;
using DropBoxMarket.Data;
using DropBoxMarket.Models;
using DropBoxMarket.Extensions; // Увери се, че това пространство имена е налично за GetObject/SetObject

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        return View(cart);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            TempData["Message"] = "Product not found.";
            return RedirectToAction("All", "Product");
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Title = product.Title,
                Price = product.Price,
                Quantity = 1
            });
        }

        HttpContext.Session.SetObject("cart", cart);
        TempData["Message"] = $"{product.Title} added to cart!";
        return RedirectToAction("All", "Product");
    }

    public IActionResult Clear()
    {
        HttpContext.Session.Remove("cart");
        TempData["Message"] = "Cart cleared.";
        return RedirectToAction("Index");
    }
}
