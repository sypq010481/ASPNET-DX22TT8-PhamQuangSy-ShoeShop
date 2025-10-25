using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using ShoeShop.Repository;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category/[action]/{id?}")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var categories = _dataContext.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            category.Slug = category.Name.Replace(" ", "-");
            var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Danh mục đã có trong database");
                return View(category);
            }
            if (category.Name == null)
            {
                TempData["error"] = "Vui lòng nhập tên danh mục!";
                return View(category);
            }
            //Lưu vào DB
            _dataContext.Add(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm danh mục thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var category = await _dataContext.Categories.FindAsync(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            category.Slug = category.Name.Replace(" ", "-");

            _dataContext.Update(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật danh mục thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Danh mục đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
