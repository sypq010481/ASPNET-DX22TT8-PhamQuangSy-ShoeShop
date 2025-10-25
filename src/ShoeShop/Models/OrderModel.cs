using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public string? OrderId { get; set; } //Mã đơn hàng
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? PaymentMethod { get; set; }        
        public int? Status { get; set; }        
        public decimal Total { get; set; } // Tổng tiền        
        public DateTime Created_at { get; set; } = DateTime.Now;

        // Chi tiết đơn hàng
        public virtual ICollection<OrderDetailModel>? OrderDetails { get; set; }
    }
}
