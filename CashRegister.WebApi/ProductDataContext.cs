using CashRegister.Shared;
using Microsoft.EntityFrameworkCore;

namespace CashRegister.WebApi
{
    public class ProductDataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Receipt> Receipts { get; set; }

        public DbSet<ReceiptLine> ReceiptLines { get; set; }

        public ProductDataContext(DbContextOptions<ProductDataContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Receipt>().HasMany(a => a.ReceiptLines).WithOne(r => r.Receipt);

            modelBuilder.Entity<ReceiptLine>().HasOne(a => a.Product).WithMany(p => p.ReceiptLines);
        }
    }
}
