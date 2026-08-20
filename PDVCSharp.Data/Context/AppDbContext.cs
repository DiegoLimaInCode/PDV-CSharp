using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Domain.Entities;
using System.Text.Json;

namespace PDVCSharp.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItemVendas { get; set; }
        public DbSet<CaixaSessao> CaixaSessoes { get; set; }
        public DbSet<MovimentoCaixa> MovimentosCaixa { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public static void Initialize(IServiceProvider serviceProvider, Func<string, string>? hashPassword = null)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.IsSqlite())
            {
                context.Database.EnsureCreated();
            }
            else
            {
                context.Database.Migrate();
            }

            Seed(context, hashPassword);
        }

        public static void Seed(AppDbContext context, Func<string, string>? hashPassword = null)
        {
            SeedUsuarios(context, hashPassword);
            SeedProdutos(context);
        }

        private static void SeedUsuarios(AppDbContext context, Func<string, string>? hashPassword)
        {
            string Hash(string senha) => hashPassword is null ? senha : hashPassword(senha);

            if (!context.Usuarios.Any(u => u.Login == "admin"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Name = "Administrador",
                    Cargo = Cargo.Administrador,
                    Login = "admin",
                    Password = Hash("admin")
                });
            }

            if (!context.Usuarios.Any(u => u.Login == "caixa"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Name = "Operador de Caixa",
                    Cargo = Cargo.Caixa,
                    Login = "caixa",
                    Password = Hash("caixa")
                });
            }

            context.SaveChanges();
        }

        private static void SeedProdutos(AppDbContext context)
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Produtos.json");
            if (!File.Exists(jsonPath))
            {
                jsonPath = "Produtos.json";
            }

            if (!File.Exists(jsonPath))
            {
                return;
            }

            var produtosBase = JsonSerializer.Deserialize<List<Produto>>(File.ReadAllText(jsonPath));
            if (produtosBase is null || produtosBase.Count == 0)
            {
                return;
            }

            var existentes = context.Produtos.Select(p => new { p.Sku, p.Name }).ToList();
            var skusExistentes = existentes.Select(p => p.Sku).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var nomesExistentes = existentes.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var produto in produtosBase)
            {
                if (string.IsNullOrWhiteSpace(produto.Sku) ||
                    skusExistentes.Contains(produto.Sku) ||
                    nomesExistentes.Contains(produto.Name))
                {
                    continue;
                }

                context.Produtos.Add(new Produto
                {
                    Name = produto.Name,
                    Sku = produto.Sku,
                    Categoria = produto.Categoria,
                    Price = produto.Price,
                    Quantity = produto.Quantity,
                    ImagePath = produto.ImagePath
                });
                skusExistentes.Add(produto.Sku);
            }

            context.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Sku).HasMaxLength(50);
                entity.Property(p => p.Categoria).HasMaxLength(80);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(p => p.Sku);
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

            modelBuilder.Entity<Venda>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(v => v.DescontoAplicado).HasColumnType("decimal(18,2)");
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)");
                entity.Property(v => v.TotalRecebido).HasColumnType("decimal(18,2)");
                entity.HasOne(v => v.CaixaSessao)
                      .WithMany()
                      .HasForeignKey(v => v.CaixaSessaoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(v => v.Itens)
                      .WithOne(i => i.Venda)
                      .HasForeignKey(i => i.VendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemVenda>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,2)");
                entity.HasOne(i => i.Produto)
                      .WithMany()
                      .HasForeignKey(i => i.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
