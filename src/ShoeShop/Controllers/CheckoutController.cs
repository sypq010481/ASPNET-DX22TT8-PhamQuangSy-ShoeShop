using Microsoft.AspNetCore.Mvc;
using ShoeShop.Extensions;
using ShoeShop.Models;
using ShoeShop.Repository;
using ShoeShop.Services.Momo;
using ShoeShop.Services.Vnpay;

namespace ShoeShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private static Random random = new Random();
        private IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        public CheckoutController(DataContext context, IMomoService momoService, IVnPayService vnPayService)
        {
            _dataContext = context;
            _momoService = momoService;
            _vnPayService = vnPayService;
        }        
        [Route("Checkout")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            Decimal subtotal = 0;
            Decimal total = 0;
            int countItem = 0;
            if (cart != null)
            {
                //Tính tổng giá tiền giỏ hàng
                foreach (var item in cart)
                {
                    var product = _dataContext.Products.Where(p => p.Id == item.Id).FirstOrDefault();
                    if (product != null)
                    {
                        if (product.Quantity > 0 && product.Quantity >= item.Quantity)
                        {
                            subtotal += item.Price * item.Quantity;
                            total += item.Price * item.Quantity;
                            countItem++;
                        }
                        else
                        {
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                    else
                    {
                        HttpContext.Session.Remove("cart");
                        return RedirectToAction("Index", "Cart");
                    }
                }
            }
            ViewData["SubTotal"] = subtotal.ToString("#,0");
            ViewData["Total"] = total.ToString("#,0");
            ViewData["CountItemCart"] = countItem;
            return View();
        }
        //Status: 1: Chưa thanh toán | 2: đã thanh toán | 3: lỗi
        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> Index(string name, string address, string email, string phone, decimal subtotal, decimal discount, decimal total, string paymentmethod)
        {            
            HttpContext.Session.SetDecimal("CustomerTotal", total);
            HttpContext.Session.SetDecimal("CustomerSubTotal", subtotal);            

            var order = new OrderModel
            {
                OrderId = DateTime.Now.ToString("ddMMyy") + RandomString(6),
                Name = name,
                Address = address,
                Email = email,
                Phone = phone,
                PaymentMethod = Convert.ToInt32(paymentmethod),                
                Status = 1,                
                Total = total,
                Created_at = DateTime.Now
            };

            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
            if (cart == null || !cart.Any())
            {
                throw new ArgumentException("Giỏ hàng không có sản phẩm!");
            }
            else
            {
                foreach (var item in cart)
                {                    
                    var product = await _dataContext.Products.FindAsync(item.Id);
                    if (product != null)
                    {
                        if (product.Quantity > 0 && product.Quantity >= item.Quantity)
                        {
                            product.Quantity = product.Quantity - item.Quantity;
                            _dataContext.Products.Update(product);
                            await _dataContext.SaveChangesAsync();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                }
            }

            var orderDetails = cart.Select(item => new OrderDetailModel
            {
                OrderId = order.Id,
                ProductId = item.Id,
                Quantity = item.Quantity,
                Total = item.Quantity * item.Price
            }).ToList();

            _dataContext.OrderDetails.AddRange(orderDetails);
            _dataContext.SaveChanges();
            HttpContext.Session.Remove("cart");            
            switch (paymentmethod)
            {
                case "1": //Thanh toán tại cửa hàng    
                    return RedirectToAction("Index", "Approve");                    
                case "2": //Momo
                    var response = await _momoService.CreatePaymentAsync(order);                    
                    return Redirect(response.PayUrl);
                case "3": //VNPay                                 
                    var url = _vnPayService.CreatePaymentUrl(order, HttpContext);                    
                    return Redirect(url);
                default:
                    return View();
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
