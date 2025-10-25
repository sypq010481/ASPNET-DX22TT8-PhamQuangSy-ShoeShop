using Microsoft.AspNetCore.Mvc;
using ShoeShop.Extensions;
using ShoeShop.Models;

namespace ShoeShop.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
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
            return View();
        }
    }
}
