using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Infrastructure.Repositories;

namespace WebApiProject.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Kết nối SQL Server (hoặc SQLite)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repository
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductTypeRepository>();
            services.AddScoped<StorageRepository>();
            services.AddScoped<StorageTypeRepository>();

            return services;
        }
    }
}
