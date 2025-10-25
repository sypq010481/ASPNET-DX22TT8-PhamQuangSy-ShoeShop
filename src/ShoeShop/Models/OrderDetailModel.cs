using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShoeShop.Models
{
    public class OrderDetailModel
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; } // Mã sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal Total { get; set; } // Tổng tiền của sản phẩm (Quantity * Price)
        [ForeignKey("OrderId")]
        public virtual OrderModel? Order { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductModel? Product { get; set; }

    }
}
