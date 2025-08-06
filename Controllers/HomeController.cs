using DropBoxMarket.Data;
using DropBoxMarket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DropBoxMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void SetCategoryCount()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
        }

        public IActionResult Index()
        {
            SetCategoryCount();
            return View();
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