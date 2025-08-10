using DropBoxMarket.Data;
using DropBoxMarket.Models;
using DropBoxMarket.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace DropBoxMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        private void SetCategoryCount()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
        }

        public async Task<IActionResult> Index(string? search)
        {
            var topCategories = await _context.Categories
                .Select(c => new CategoryCard
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .OrderByDescending(c => c.ProductCount)
                .Take(6)
                .ToListAsync();

            var featured = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(8)
                .ToListAsync();

            var userId = _userManager.GetUserId(User);

            var vm = new HomeViewModel
            {
                TopCategories = topCategories,
                FeaturedProducts = featured,
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalOrdersForUser = userId == null
                    ? 0
                    : await _context.Orders.CountAsync(o => o.UserId == userId),
                SearchTerm = search
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            SetCategoryCount();
            return View();
        }

        public IActionResult About()
        {
            SetCategoryCount();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            SetCategoryCount();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult TestError()
        //{
        //    throw new Exception("Test 500 error");
        //}
    }


}