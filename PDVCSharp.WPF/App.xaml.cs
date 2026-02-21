using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Extensions;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Context;
using WpfApplication = System.Windows.Application;

// 💡 DICA: "using X = Y" cria um apelido (alias) para evitar conflitos de nome.
//    Aqui, "WpfApplication" é um apelido para System.Windows.Application,
//    porque o nome "Application" poderia conflitar com outros namespaces.

namespace PDVCSharp.WPF
{
    /// <summary>
    /// App.xaml.cs é o PONTO DE ENTRADA da aplicação WPF.
    /// Aqui configuramos tudo que precisa estar pronto antes das telas aparecerem.
    /// </summary>
    public partial class App : WpfApplication
    {
        // ServiceProvider = contêiner de injeção de dependência (DI)
        // É estático para ser acessível de qualquer lugar do app
        // "null!" = informa ao compilador que será inicializado antes do uso
        //
        // 💡 DICA: O ServiceProvider é como uma "fábrica" que cria objetos automaticamente
        //    e injeta suas dependências. Ex: ao pedir AuthService, ele cria o UserRepository
        //    e o AppDbContext automaticamente.
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        // OnStartup é chamado quando o app inicia (antes de qualquer janela abrir)
        protected override void OnStartup(StartupEventArgs e)
        {
            // Cria a coleção de serviços (lista de tudo que o app vai precisar)
            var services = new ServiceCollection();

            // Registra os serviços em cada camada:
            services.DatabaseConnection(); // Configura o banco MySQL (AppDbContext)
            services.AddRepositories();    // Registra os repositórios (UserRepository, etc.)
            services.AddServices();        // Registra os serviços de negócio (AuthService, etc.)

            // Constrói o contêiner de DI — a partir daqui, podemos "pedir" serviços
            ServiceProvider = services.BuildServiceProvider();

            // Inicializa o banco: cria tabelas e insere dados padrão (seed)
            AppDbContext.Initialize(ServiceProvider);

            // Chama o OnStartup original do WPF (abre a MainWindow)
            base.OnStartup(e);
        }
    }

}
