using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Extensions;
using ShoeShop.Models;
using ShoeShop.Repository;

namespace ShoeShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {
            var categories = _dataContext.Categories.ToList();
            var products = _dataContext.Products.Include(p => p.Category).ToList();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            int countItem = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    countItem++;
                }
            }
            ViewData["CountItemCart"] = countItem;

            ViewBag.Categories = categories;
            ViewBag.Products = products;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
