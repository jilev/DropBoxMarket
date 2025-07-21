using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropBoxMarket.Data;
using DropBoxMarket.Models;
using DropBoxMarket.Models.ViewModels;
using DropBoxMarket.Extensions;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart == null || !cart.Any())
        {
            TempData["Message"] = "Cart is empty!";
            return RedirectToAction("Index", "Cart");
        }

        var model = new CheckoutViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart == null || !cart.Any())
        {
            TempData["Message"] = "Cart is empty!";
            return RedirectToAction("Index", "Cart");
        }

        var userId = _userManager.GetUserId(User);

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            FullName = model.FullName,
            Phone = model.Phone,
            Address = model.Address,
            Items = cart.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("cart");
        TempData["Message"] = "Order placed successfully!";
        return RedirectToAction("Confirmation");
    }

    public IActionResult MyOrders()
    {
        var userId = _userManager.GetUserId(User);
        var orders = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        return View(orders);
    }
    public IActionResult Confirmation()
    {
        return View();
    }

}
