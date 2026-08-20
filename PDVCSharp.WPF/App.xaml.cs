using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Extensions;
using PDVCSharp.Application.Security;
using PDVCSharp.Data.Context;
using PDVCSharp.WPF.extensions;
using WpfApplication = System.Windows.Application;

namespace PDVCSharp.WPF
{
    public partial class App : WpfApplication
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.DatabaseConnection(configuration);
            services.AddRepositories();
            services.AddServices();
            services.AddviewModel();

            ServiceProvider = services.BuildServiceProvider();
            AppDbContext.Initialize(ServiceProvider, PasswordHasher.Hash);

            base.OnStartup(e);
        }
    }
}
