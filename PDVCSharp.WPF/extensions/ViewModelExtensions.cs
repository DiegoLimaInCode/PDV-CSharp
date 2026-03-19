using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.ViewModels;
namespace PDVCSharp.WPF.extensions
{
    public static class ViewModelExtensions
    {
        public static IServiceCollection AddviewModel(this IServiceCollection services)
        {
            services.AddTransient<FechamentoViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<VendaViewModel>();
            services.AddTransient<AberturaViewModel>();

            return services;
        }
    }
}
