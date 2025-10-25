using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using ShoeShop.Repository;
using System.Diagnostics;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product/[action]/{id?}")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnviroment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            var products = _dataContext.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name");            

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel Product)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            Product.Slug = Product.Name.Replace(" ", "-");
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", Product.CategoryId);

            var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == Product.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "Sản phẩm đã có trong database");
                return View(Product);
            }
            if (Product.Name == null)
            {
                TempData["error"] = "Vui lòng nhập tên sản phẩm!";
                return View(Product);
            }
            if (Product.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                //Nếu chưa có thư mục thì tạo thư mục
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                string imageName = Guid.NewGuid().ToString() + "_" + Product.Image.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await Product.Image.CopyToAsync(fs);
                }

                Product.ImageUrl = imageName;
            }
            //Lưu vào DB
            _dataContext.Add(Product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm sản phẩm thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            var category = await _dataContext.Products
            .Include(c => c.Category)
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == id);

            ViewBag.productsize = category.ProductSizes.ToList();
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", category.CategoryId);

            return View(category);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            product.Slug = product.Name.Replace(" ", "-");
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            var productInDb = await _dataContext.Products.FindAsync(product.Id);

            productInDb.Name = product.Name;
            productInDb.Slug = product.Slug;
            productInDb.Description = product.Description;
            productInDb.Price = product.Price;
            productInDb.Quantity = product.Quantity;
            productInDb.CategoryId = product.CategoryId;
            productInDb.Content = product.Content;
            productInDb.Status = product.Status;


            if (product.Image != null)
            {
                Debug.WriteLine("This is a debug message.");
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                //Check đã có folder updoad chưa nếu chưa thì tạo folder mới
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                ////Xóa hình cũ đi
                if (product.ImageUrl != null)
                {
                    string oldfilePath = Path.Combine(uploadsDir, product.ImageUrl);
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                }

                //Upload hình mới
                string imageName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(fs);
                }

                productInDb.ImageUrl = imageName;
            }
            //_dataContext.Update(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật sản phẩm thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var product = await _dataContext.Products.FindAsync(Id);
            if (product.ImageUrl != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                string oldfilePath = Path.Combine(uploadsDir, product.ImageUrl);
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(ProductSizeModel productsize)
        {
            bool exists = await _dataContext.ProductSize
                .AnyAsync(x => x.ProductId == productsize.ProductId && x.size == productsize.size);
            if (exists)
            {
                TempData["error"] = "Size này đã tồn tại cho sản phẩm này!";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }            
            _dataContext.Add(productsize);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm thành công";
            return Redirect(HttpContext.Request.Headers["Referer"]);                                   
        }

        [HttpGet]
        public IActionResult EditModal(int id)
        {
            var result = _dataContext.ProductSize.Where(p => p.Id == id).FirstOrDefault();
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(ProductSizeModel productsize)
        {
            bool exists = await _dataContext.ProductSize
                .AnyAsync(x =>
                    x.ProductId == productsize.ProductId &&
                    x.size == productsize.size &&
                    x.Id != productsize.Id
                );

            if (exists)
            {
                TempData["error"] = "Size này đã tồn tại cho sản phẩm này!";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

            var productSize = await _dataContext.ProductSize.FindAsync(productsize.Id);
            productSize.size = productsize.size;
            productSize.Quantity = productsize.Quantity;
            _dataContext.Update(productSize);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật thành công";
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(int? Id)
        {
            var product = await _dataContext.ProductSize.FindAsync(Id);            
            _dataContext.ProductSize.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thành công";
            return Json(product);
        }
    }
}
