using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using ShoeShop.Repository;
using System;
using System.Diagnostics;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Order/[action]/{id?}")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        private static Random random = new Random();

        public OrderController(DataContext context)
        {
            _dataContext = context;            
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            var orders = _dataContext.Orders.OrderByDescending(p => p.Id).ToList();
            return View(orders);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderModel Order)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            //Tạo mã đơn hàng theo định dạnh Năm Tháng Ngày và 6 ký tự ngẫu nhiên
            Order.OrderId = DateTime.Now.ToString("yyMMdd") + RandomString(6);
            //Lưu vào DB
            _dataContext.Add(Order);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm đơn thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            var order = await _dataContext.Orders            
            .Include(p => p.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

            ViewBag.orderDetails = order?.OrderDetails?.ToList();
            ViewBag.products = new SelectList(_dataContext.Products, "Id", "Name");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderModel Order)
        {
            var name = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }            
            _dataContext.Update(Order);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật đơn hàng thành công";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var order = await _dataContext.Orders.FindAsync(Id);
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đơn hàng đã được xóa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(OrderDetailModel orderdetail)
        {

            var product = await _dataContext.Products.FindAsync(orderdetail.ProductId);
            orderdetail.Total = product.Price * orderdetail.Quantity;
            _dataContext.OrderDetails.Add(orderdetail);
            await _dataContext.SaveChangesAsync();

            await UpdateOrderTotalAsync(orderdetail.OrderId);

            TempData["success"] = "Thêm thành công";
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [HttpGet]
        public IActionResult EditModal(int id)
        {            
            var result = _dataContext.OrderDetails.FirstOrDefault(p => p.Id == id);
            var products = _dataContext.Products
                .Select(x => new { value = x.Id, text = x.Name })
                .ToList();

            return Json(new
            {
                id = result.Id,
                productid = result.ProductId,
                quantity = result.Quantity,
                total = result.Total,
                products = products
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(OrderDetailModel orderdetail)
        {            
            var dbDetail = await _dataContext.OrderDetails
                .FirstOrDefaultAsync(x => x.Id == orderdetail.Id);            
            var product = await _dataContext.Products.FindAsync(orderdetail.ProductId);

            // Cập nhật
            dbDetail.ProductId = orderdetail.ProductId;
            dbDetail.Quantity = orderdetail.Quantity;
            dbDetail.Total = product.Price * orderdetail.Quantity;

            await _dataContext.SaveChangesAsync();

            await UpdateOrderTotalAsync(dbDetail.OrderId);

            TempData["success"] = "Cập nhật thành công";
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(int? Id)
        {
            var order = await _dataContext.OrderDetails.FindAsync(Id);
            _dataContext.OrderDetails.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thành công";
            return Json(order);
        }

        public IActionResult CreateModal()
        {
            ViewBag.products = new SelectList(_dataContext.Products, "Id", "Name");
            return PartialView();
        }

        public IActionResult EditModal()
        {
            ViewBag.products = new SelectList(_dataContext.Products, "Id", "Name");
            return PartialView();
        }

        private async Task UpdateOrderTotalAsync(int orderId)
        {
            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.Total = await _dataContext.OrderDetails
                    .Where(d => d.OrderId == orderId)
                    .SumAsync(d => d.Total);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
