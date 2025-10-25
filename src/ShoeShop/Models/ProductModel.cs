using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Models
{    
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống tên sản phẩm")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        [DefaultValue(0)]
        public int Price { get; set; }
        [DefaultValue(0)]        
        public string Slug { get; set; }
        [DefaultValue(1)]
        public int Status { get; set; }        

        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual required CategoryModel? Category { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public virtual ICollection<ProductSizeModel>? ProductSizes { get; set; }
        [DefaultValue(0)]
        public int Quantity { get; set; }

    }
}
