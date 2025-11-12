using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
using WebApiProject.Domain.Entities;

namespace WebApiProject.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User> 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductType> ProductTypes => Set<ProductType>();
        public DbSet<Storage> Storages => Set<Storage>();
        public DbSet<StorageType> StorageTypes => Set<StorageType>();
        public DbSet<User> Users => Set<User>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}