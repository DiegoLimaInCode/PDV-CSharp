using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PDVCSharp.Data.Context
{
    // Factory usada APENAS em tempo de design (quando você roda "dotnet ef migrations")
    // ?? DICA: IDesignTimeDbContextFactory só é usada pelo comando "dotnet ef".
    //    Em runtime, o DbContext é criado pela injeção de dependência.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Connection string do MySQL
            // ?? DICA: Em produção, use variáveis de ambiente ou User Secrets
            var connectionString = "Server=localhost;Database=pdvcsharp;User=root;Password=1234;";

            // Configura o EF Core para usar MySQL via Pomelo
            // ServerVersion.AutoDetect = detecta automaticamente a versão do MySQL
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
