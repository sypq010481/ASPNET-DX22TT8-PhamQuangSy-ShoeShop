using ShoeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoeShop.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }        
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProductSizeModel> ProductSize { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<ContactModel> Contact { get; set; }
    }
}
