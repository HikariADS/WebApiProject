using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Infrastructure.Repositories;
using WebApiProject.Domain.Entities;
using WebApiProject.Application.IServices;
using WebApiProject.Application.Services;
using Microsoft.AspNetCore.Identity;

namespace WebApiProject.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>() 
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductTypeRepository>();
            services.AddScoped<StorageRepository>();
            services.AddScoped<StorageTypeRepository>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
