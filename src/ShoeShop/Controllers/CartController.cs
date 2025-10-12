using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
