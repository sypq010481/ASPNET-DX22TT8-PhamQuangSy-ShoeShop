using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Contact")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
