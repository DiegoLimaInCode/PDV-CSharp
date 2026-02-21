using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Context;
using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Extensions
{
    // Classe estática com métodos de extensão para configurar a injeção de dependência
    // 💡 DICA: Métodos de extensão "estendem" uma classe existente sem modificá-la.
    //    O "this" antes do parâmetro permite chamar: services.DatabaseConnection()
    public static class ServiceExtensions
    {
        // Configura a conexão com o banco de dados MySQL
        public static IServiceCollection DatabaseConnection(this IServiceCollection services)
        {
            // Connection string: endereço e credenciais do banco MySQL
            // 💡 DICA: Em produção, mova para appsettings.json ou variáveis de ambiente
            var connectionString = "Server=localhost;Database=pdvcsharp;User=root;Password=1234;";

            // Registra o AppDbContext no contêiner de DI com a configuração do MySQL
            // UseMySql = configura o provider Pomelo para MySQL
            // ServerVersion.AutoDetect = detecta a versão do MySQL automaticamente
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            return services; // Retorna para permitir encadeamento (fluent API)
        }

        // Registra os repositórios no contêiner de injeção de dependência
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // AddScoped<Interface, Implementação> = quando alguém pedir IUserRepository,
            //   o contêiner cria um UserRepository automaticamente
            // 💡 DICA: Scoped = uma nova instância por escopo de DI.
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        // Registra os serviços da camada de aplicação
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Como AuthService depende de IUserRepository, o contêiner injeta automaticamente
            services.AddScoped<AuthService>();
            return services;
        }
    }
}
