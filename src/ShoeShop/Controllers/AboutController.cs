using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
