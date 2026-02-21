using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Data.Context
{
    // AppDbContext é o "coração" do Entity Framework — representa o banco de dados na aplicação.
    // Herda de DbContext, que é a classe base do EF Core para acesso ao banco.
    // ?? DICA: Cada DbSet<T> representa uma TABELA no banco de dados.
    //    O EF Core traduz suas consultas C# (LINQ) em SQL automaticamente.
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }   // Tabela "Produtos" no MySQL
        public DbSet<Usuario> Usuarios { get; set; }   // Tabela "Usuarios" no MySQL

        // Construtor que recebe as opções de configuração (connection string, provider, etc.)
        // O ": base(options)" repassa as opções para a classe pai (DbContext)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Método estático que inicializa o banco com dados padrão (seed data)
        // É chamado uma vez quando o app inicia (em App.xaml.cs)
        // ?? DICA: "static" significa que não precisa criar uma instância para chamar.
        //    Você chama assim: AppDbContext.Initialize(serviceProvider)
        public static void Initialize(IServiceProvider serviceProvider)
        {
            // Cria um "escopo" de injeção de dependência para obter o DbContext
            // ?? DICA: O "using" garante que o escopo será descartado após o uso (libera memória)
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Cria o banco de dados e as tabelas se ainda não existirem
            // ?? DICA: Em produção, prefira usar Migrations (dotnet ef migrations)
            context.Database.EnsureCreated();

            // Se não existir nenhum usuário, cria o usuário padrão "admin"
            if (!context.Usuarios.Any())
            {
                var defaultUser = new Usuario
                {
                    Login = "admin",
                    Password = "admin"
                };
                context.Usuarios.Add(defaultUser);   // Adiciona ao contexto (ainda em memória)
                context.SaveChanges();                // Salva no banco (executa o INSERT)
            }
        }

        // OnModelCreating configura como as entidades são mapeadas para as tabelas
        // ?? DICA: Isso se chama "Fluent API" — uma alternativa aos Data Annotations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);                                    // Chave primária
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200); // Obrigatório, máx 200 chars
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");// 18 dígitos, 2 decimais
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);                                        // Chave primária
                entity.Property(u => u.Login).IsRequired().HasMaxLength(100);    // Obrigatório
                entity.Property(u => u.Password).IsRequired().HasMaxLength(200); // Obrigatório
            });
        }
    }
}
