using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Infrastructure.Repositories;

namespace WebApiProject.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Kết nối SQL Server (hoặc SQLite tùy bạn)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký repository vào DI
            services.AddScoped<ProductRepository>();
            services.AddScoped<StorageRepository>();
            services.AddScoped<ProductTypeRepository>();
            services.AddScoped<StorageTypeRepository>();

            return services;
        }
    }
}
