using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropBoxMarket.Data;
using DropBoxMarket.Models;
using DropBoxMarket.Models.ViewModels;
using DropBoxMarket.Extensions;
using Microsoft.AspNetCore.SignalR;
using DropBoxMarket.Hubs;
using System.Linq;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<OrderHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
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

        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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

        await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOrderNotification", new
        {
            Message = $"Нова поръчка от {model.FullName}",
            OrderId = order.Id,
            Date = order.OrderDate.ToString("dd.MM.yyyy HH:mm")
        });

        HttpContext.Session.Remove("cart");
        TempData["Message"] = "Order placed successfully!";

        return RedirectToAction("Confirmation", new { orderId = order.Id });
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

    public IActionResult Confirmation(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
            return NotFound();

        var model = new ConfirmationViewModel
        {
            FullName = order.FullName,
            OrderDate = order.OrderDate,
            TotalPrice = order.Items.Sum(i => i.Quantity * i.Product.Price),
            Items = order.Items.Select(i => new OrderItemViewModel
            {
                ProductName = i.Product.Title,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList()
        };

        return View(model);
    }
}
