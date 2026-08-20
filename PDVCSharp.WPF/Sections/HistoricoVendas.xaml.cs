using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class HistoricoVendas : UserControl, IScreenActivation
    {
        private readonly HistoricoVendasViewModel _vm;

        public HistoricoVendas()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<HistoricoVendasViewModel>();
            DataContext = _vm;
        }

        public void OnNavigatedTo() => _ = _vm.CarregarAsync();

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
            => MainWindow.Navigation.Navigate(AppScreen.CaixaLivre);
    }
}
