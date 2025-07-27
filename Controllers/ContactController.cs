using Microsoft.AspNetCore.Mvc;
using DropBoxMarket.Models;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]  
    public IActionResult Index(ContactFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData["Message"] = "Thank you for your message! We'll get back to you soon.";
        return RedirectToAction("Index");
    }
}
