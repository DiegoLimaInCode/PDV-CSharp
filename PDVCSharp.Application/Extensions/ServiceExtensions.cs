using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Context;
using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection DatabaseConnection(this IServiceCollection services)
        {
            var connectionString = "Server=localhost;Database=pdvcsharp;User=root;Password=1234;";

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<VendaService>();
            services.AddScoped<VendaFinalService>();
            services.AddTransient<FechamentoService>();
            return services;
        }
    }
}
