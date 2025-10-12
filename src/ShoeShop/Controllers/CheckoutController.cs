using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
