using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
