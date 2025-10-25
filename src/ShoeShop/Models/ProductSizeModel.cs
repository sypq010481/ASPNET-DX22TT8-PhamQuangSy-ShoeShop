using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Models
{
    public class ProductSizeModel
    {
        [Key]
        public int Id { get; set; }
        public string? size { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual required ProductModel? Product { get; set; }       
    }
}
