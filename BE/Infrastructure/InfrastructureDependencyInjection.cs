using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiProject.Infrastructure.Persistence;
using WebApiProject.Infrastructure.Repositories;
using WebApiProject.Domain.Entities;
using WebApiProject.Application.IServices;
using WebApiProject.Application.Services;
using Microsoft.AspNetCore.Identity;
using WebApiProject.Application.IRepositories;

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

            // FIXED: Register repository interface bindings
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<IStorageTypeRepository, StorageTypeRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<WebApiProject.Application.IRepositories.IPendingRegistrationRepository, PendingRegistrationRepository>();
            services.AddScoped<IPasswordResetCodeRepository, PasswordResetCodeRepository>();

            // Paging + Services
            services.AddScoped(typeof(IGenericPagingService<,>), typeof(GenericPagingService<,>));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IStorageTypeService, StorageTypeService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}