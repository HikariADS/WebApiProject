using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Domain.Entities;


namespace WebApiProject.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var rnd = new Random();
            // await context.Database.MigrateAsync(); // It's better to run migrations via command line
            if (!context.ProductTypes.Any())
            {
                var types = new List<ProductType>
                {
                    new() { Name = "Electronics"},
                    new() { Name = "Toys"},
                    new() { Name = "Clothing"},
                    new() { Name = "Accesories"},
                    new() { Name = "Sports"}
                };
                context.ProductTypes.AddRange(types);
                await context.SaveChangesAsync();
            }
            if (!context.Products.Any())
            {
                var types = context.ProductTypes.ToList();
                var products = Enumerable.Range(1, 30)
                .Select(i => new Product
                {
                    Name = $"Product {i}",
                    Description = $"Sample description for product {i}",
                    Price = rnd.Next(50, 1000),
                    ProductTypeId = types[rnd.Next(types.Count)].Id, 
                    Quantity = rnd.Next(1, 100)                  
                })
                .ToList();
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
            if (!context.StorageTypes.Any())
            {
                var storageTypes = new List<StorageType>
                {
                    new() { Name = "Main Warehouse" },
                    new() { Name = "Backup Storage" },
                    new() { Name = "Showroom" }
                };
                context.StorageTypes.AddRange(storageTypes);
                await context.SaveChangesAsync();
            }

            var roles = new[] { "Admin", "Manager", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@local.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@local.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("user@local.com") == null)
            {
                var user = new User
                {
                    UserName = "user",
                    Email = "user@local.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }

            var adminUser = await userManager.FindByEmailAsync("admin@local.com");
            if (!context.Storages.Any() && adminUser != null)
            {
                var productIds = await context.Products.Select(p => p.Id).ToListAsync();
                var storageTypeIds = await context.StorageTypes.Select(st => st.Id).ToListAsync();
                
                if (productIds.Any() && storageTypeIds.Any() && adminUser != null)
                {
                var storages = Enumerable.Range(1, 10)
                    .Select(i => new Storage
                    {
                            ProductId = productIds[rnd.Next(productIds.Count)],
                            Quantity = rnd.Next(1, 50),
                            UserId = adminUser.Id, // dùng đúng kiểu string
                            StorageTypeId = storageTypeIds[rnd.Next(storageTypeIds.Count)],
                        ImportDate = DateTimeOffset.Now.AddDays(-rnd.Next(10)),
                        ExportDate = DateTimeOffset.Now
                    })
                    .ToList();
                    context.Storages.AddRange(storages);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

}