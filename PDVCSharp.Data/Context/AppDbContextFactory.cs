using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PDVCSharp.Data.Context
{
    // Factory usada APENAS em tempo de design (quando você roda "dotnet ef migrations")
    // O EF Core precisa criar o DbContext para gerar migrations, mas nesse momento
    // o app não está rodando — então ele usa essa factory para saber como criar o contexto.
    //
    // 💡 DICA: IDesignTimeDbContextFactory só é usada pelo comando "dotnet ef".
    //    Em runtime (quando o app roda), o DbContext é criado pela injeção de dependência.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        // Método que cria o AppDbContext com a connection string do MySQL
        public AppDbContext CreateDbContext(string[] args)
        {
            // Builder que monta as opções de configuração do DbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Connection string do MySQL — define servidor, banco, usuário e senha
            // 💡 DICA: Em produção, use variáveis de ambiente ou User Secrets
            //    para não expor senhas no código fonte.
            var connectionString = "Server=localhost;Database=pdvcsharp;User=root;Password=1234;";

            // Configura o EF Core para usar MySQL via Pomelo
            // ServerVersion.AutoDetect = detecta automaticamente a versão do MySQL
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            // Retorna o contexto configurado
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
