using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DropBoxMarket.Data;
using DropBoxMarket.Models;

namespace DropBoxMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == id);

            return order == null ? NotFound() : View(order);
        }
    }
}
