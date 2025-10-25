using Microsoft.AspNetCore.Mvc;
using ShoeShop.Repository;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
