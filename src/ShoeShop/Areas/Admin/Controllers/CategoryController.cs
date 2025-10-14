using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category/[action]")]
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {            
            return View();
        }

        public IActionResult Edit()
        {            
            return View();
        }
    }
}
