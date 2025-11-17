using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Infrastructure.Presistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();
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
                var rnd = new Random();
                var types = context.ProductTypes.Tolist();
                var products = Enumerable.Range(1, 30)
                .Select(i => new ProductType
                {
                    Name = $"Product {i}",
                    Description = $"Sapmple description fo product {i}",
                    Price = rnd.Next(50, 1000),
                    ProductTypeId = types[rnd.Next(types.Count)].Id                    
                })
                .ToList();
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
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

            if (!context.Storages.Any())
            {
                var rnd = new Random();
                var storages = Enumerable.Range(1, 10)
                    .Select(i => new Storage
                    {
                        ProductId = rnd.Next(1, 30),
                        Quantity = rnd.Next(1, 50),
                        UserId = 0, // nếu bạn chưa dùng Identity, để 0 hoặc null
                        StorageTypeId = rnd.Next(1, 3),
                        ImportDate = DateTimeOffset.Now.AddDays(-rnd.Next(10)),
                        ExportDate = DateTimeOffset.Now
                    })
                    .ToList();

                context.Storages.AddRange(storages);
                await context.SaveChangesAsync();
            }

            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ Seed Admin user
            if (await userManager.FindByEmailAsync("admin@local.com") == null)
            {
                var admin = new IdentityUser
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
                var user = new IdentityUser
                {
                    UserName = "user",
                    Email = "user@local.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }
            }
        }
    }

}