using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Context;
using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection DatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
                ?? Environment.GetEnvironmentVariable("PDV_CONNECTION")
                ?? "Data Source=pdv.db";

            services.AddDbContext<AppDbContext>(options =>
            {
                if (EhSqlite(connectionString))
                {
                    options.UseSqlite(connectionString);
                }
                else
                {
                    options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql"));
                }
            });

            return services;
        }

        public static bool EhSqlite(string connectionString)
            => connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
               && !connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase);

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
            services.AddScoped<CaixaService>();
            services.AddScoped<UsuarioService>();
            services.AddScoped<EstoqueService>();
            services.AddScoped<CatalogoService>();
            services.AddScoped<HistoricoVendaService>();
            return services;
        }
    }
}
