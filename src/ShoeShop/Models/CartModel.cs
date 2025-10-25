namespace ShoeShop.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalOrder { get; set; }
        public decimal Total => Quantity * Price; // Tổng giá trị        
    }
}
