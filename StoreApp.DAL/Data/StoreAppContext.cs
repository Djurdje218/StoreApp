using Microsoft.EntityFrameworkCore;
using DAL.Entities;

namespace DAL.Data
{
    public class StoreAppContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }

        public StoreAppContext(DbContextOptions<StoreAppContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>().HasKey(s => s.Code);
            modelBuilder.Entity<Product>().HasKey(p => p.id);
            modelBuilder.Entity<Product>()
                        .HasOne<Store>()
                        .WithMany()
                        .HasForeignKey(p => p.StoreCode);
        }
    }
}
