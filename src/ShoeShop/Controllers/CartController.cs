using ShoeShop.Models;
using Microsoft.AspNetCore.Mvc;
using ShoeShop.Extensions;
using ShoeShop.Models;
using ShoeShop.Repository;

namespace ShoeShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {            
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            Decimal subtotal = 0;
            Decimal total = 0;
            int countItem = 0;
            if (cart == null)
            {
                cart = new List<CartModel>();
            }
            else
            {
                //Tính tổng giá tiền giỏ hàng
                foreach (var item in cart)
                {
                    subtotal += item.Price * item.Quantity;
                    total += item.Price * item.Quantity;
                    countItem++;
                }
            }
            HttpContext.Session.SetInt32("subtotal", Convert.ToInt32(subtotal));
            HttpContext.Session.SetInt32("total", Convert.ToInt32(total));

            //HttpContext.Session.SetString("Userid", user.Id.ToString());
            ViewData["SubTotal"] = subtotal.ToString("#,0");
            ViewData["Total"] = total.ToString("#,0");
            ViewData["CountItemCart"] = countItem;
            return View(cart);
        }

        public IActionResult AddToCart(int Id, string Name, decimal Price, int quantity, int Type, string imageUrl)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            //Nếu giỏ hàng trống -> thêm sản phẩm vào giỏ hàng
            if (cart == null)
            {
                cart = new List<CartModel>();
            }

            //Check xem sản phảm đó đã có trong giỏ hàng hay chưa?
            var item = cart.FirstOrDefault(c => c.Id == Id);
            //Nếu chưa có sản phẩm -> thêm sản phẩm vào giỏ hàng
            if (item == null)
            {
                cart.Add(new CartModel
                {
                    Id = Id,
                    Name = Name,
                    Price = Price,
                    Quantity = quantity,
                    ImageUrl = imageUrl
                });
            }//Nếu đã có sản phẩm -> Tăng số lượng sản phẩm
            else
            {
                item.Quantity++;
            }
            HttpContext.Session.SetObjectAsJson("cart", cart);
            if (Type == 1)
            {
                return Json(true);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string action)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            var item = cart.FirstOrDefault(c => c.Id == productId);
            if (item != null)
            {
                if (action == "increase")
                {
                    item.Quantity++;
                }
                else if (action == "decrease" && item.Quantity > 1)
                {
                    item.Quantity--;
                }

                var product = _dataContext.Products.Find(productId);
                item.ImageUrl = product.ImageUrl;
                item.Price = product.Price;
                item.Name = product.Name;

            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            ViewData["SubTotal"] = 0;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveCart(int id)
        {
            System.Diagnostics.Debug.WriteLine(id);
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            var itemCart = cart.FirstOrDefault(c => c.Id == id);
            if (itemCart != null)
            {
                cart.Remove(itemCart);
            }
            HttpContext.Session.SetObjectAsJson("cart", cart);
            ViewData["SubTotal"] = 0;
            return RedirectToAction("Index");
        }
    }
}
