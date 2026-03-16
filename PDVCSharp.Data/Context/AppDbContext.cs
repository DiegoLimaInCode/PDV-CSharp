using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }   // Tabela "Produtos" no MySQL
        public DbSet<Usuario> Usuarios { get; set; }   // Tabela "Usuarios" no MySQL
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } // Tabela "MovimentacoesEstoque" no MySQL
        public DbSet<Venda> Vendas { get; set; }

        public DbSet<ItemVenda> ItemVendas { get; set; } 
        // Construtor que recebe as op��es de configura��o (connection string, provider, etc.)
        // O ": base(options)" repassa as op��es para a classe pai (DbContext)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.EnsureCreated();

            if (!context.Produtos.Any())
            {
                var repo = new ProductRepository(context);
                repo.CarregarProdutos();
            }

            if (!context.Usuarios.Any())
            {
                var defaultUser = new Usuario
                {
                    Name = "Administrador",
                    Cargo = Cargo.Administrador,
                    Login = "admin",
                    Password = "admin"
                };
                context.Usuarios.Add(defaultUser);
                context.SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Cargo).IsRequired();
                entity.Property(u => u.Login).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<MovimentacaoEstoque>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.ProdutoNome).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Motivo).HasMaxLength(500);
            });

            modelBuilder.Entity<CaixaSessao>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.ValorAbertura).HasColumnType("decimal(18,2)");
                entity.HasOne(c => c.Usuario)
                      .WithMany()
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MovimentoCaixa>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Valor).HasColumnType("decimal(18,2)");
                entity.Property(m => m.Observacao).HasMaxLength(500);
                entity.Property(m => m.LoginOperador).IsRequired().HasMaxLength(100);

                entity.HasOne(m => m.CaixaSessao)
                      .WithMany(c => c.Movimentos)
                      .HasForeignKey(m => m.CaixaSessaoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
