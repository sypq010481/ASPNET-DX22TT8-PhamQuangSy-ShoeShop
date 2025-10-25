using ShoeShop.Models;
using ShoeShop.Models.Vnpay;
namespace ShoeShop.Services.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(OrderModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
