using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Extensions;
using ShoeShop.Models;
using ShoeShop.Repository;

namespace ShoeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            string category = HttpContext.Request.Query["category"].ToString();
            string search = HttpContext.Request.Query["search"].ToString();                        
            var productsByCategory = _dataContext.Products.Include(c => c.Category).Where(p => p.Status != 0);
            if (!string.IsNullOrEmpty(category))
            {                               
                CategoryModel categoryModel = _dataContext.Categories.Where(c => c.Slug == category).FirstOrDefault();
                if (categoryModel == null) return RedirectToAction("Index");
                productsByCategory = productsByCategory.Where(p => p.CategoryId == categoryModel.Id);
            }

            if (!string.IsNullOrEmpty(search))
            {
                System.Diagnostics.Debug.WriteLine(search);
                productsByCategory = productsByCategory.Where(p => p.Name.Contains(search));
            }
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

            return View(await productsByCategory.ToListAsync());
        }

        public IActionResult Detail(int? Id)
        {
            if (Id == null) return RedirectToAction("Index");
            var Size = _dataContext.ProductSize.Include(t => t.Product).Where(p => p.ProductId == Id).ToList();
            var productsById = _dataContext.Products.Include(c => c.Category).Where(p => p.Id == Id).FirstOrDefault();

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

            ViewBag.Size = Size;
            return View(productsById);
        }
    }
}
