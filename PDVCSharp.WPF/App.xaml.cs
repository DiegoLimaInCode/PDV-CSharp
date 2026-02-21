using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Extensions;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Context;
using WpfApplication = System.Windows.Application;

namespace PDVCSharp.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.DatabaseConnection();
            services.AddRepositories();
            services.AddServices();

            ServiceProvider = services.BuildServiceProvider();

            AppDbContext.Initialize(ServiceProvider);

            base.OnStartup(e);
        }
    }

}
