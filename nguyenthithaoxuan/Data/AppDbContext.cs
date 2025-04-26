using nguyenthithaoxuan.Models;
using Microsoft.EntityFrameworkCore;

namespace nguyenthithaoxuan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderTable> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình decimal cho các bảng
            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderTable>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>(b =>
            {
                b.Property(p => p.Price)
                    .HasPrecision(18, 2);

                b.Property(p => p.SalePrice)
                    .HasPrecision(18, 2);
            });

            // Nếu bạn dùng IsDeleted cho soft delete
            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);
        }
        }
}
