using Microsoft.AspNetCore.Mvc;
using ShoeShop.Repository;

namespace ShoeShop.Controllers
{
    public class ApproveController : Controller
    {
        private readonly DataContext _dataContext;
        public ApproveController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            //Get Param Url
            string partnerCode = HttpContext.Request.Query["partnerCode"].ToString();
            if (!string.IsNullOrEmpty(partnerCode))
            {
                string errorCode = HttpContext.Request.Query["errorCode"].ToString();
                string orderId = HttpContext.Request.Query["orderId"].ToString();

                if (!string.IsNullOrEmpty(orderId))
                {
                    var order = _dataContext.Orders.Where(p => p.OrderId == orderId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(errorCode))
                    {
                        if (errorCode != "0")
                        {
                            order.Status = 3;
                            _dataContext.Orders.Update(order);
                            await _dataContext.SaveChangesAsync();
                            return View();
                        }
                        else
                        {
                            order.Status = 2;
                            _dataContext.Orders.Update(order);
                            await _dataContext.SaveChangesAsync();
                            return View();
                        }
                    }
                }
            }
            else
            {
                string vnp_ResponseCode = HttpContext.Request.Query["vnp_ResponseCode"].ToString();
                string vnp_TransactionStatus = HttpContext.Request.Query["vnp_TransactionStatus"].ToString();
                string vnp_TxnRef = HttpContext.Request.Query["vnp_TxnRef"].ToString();

                if (!string.IsNullOrEmpty(vnp_TxnRef))
                {
                    if (!string.IsNullOrEmpty(vnp_ResponseCode) && !string.IsNullOrEmpty(vnp_TransactionStatus))
                    {
                        var orderResult = _dataContext.Orders.Where(p => p.OrderId == vnp_TxnRef).FirstOrDefault();
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            orderResult.Status = 2;
                            _dataContext.Orders.Update(orderResult);
                            await _dataContext.SaveChangesAsync();
                            return View();
                        }
                        else
                        {
                            orderResult.Status = 3;
                            _dataContext.Orders.Update(orderResult);
                            await _dataContext.SaveChangesAsync();
                            return View();
                        }

                    }
                }
            }
            return View();
        }
    }
}
