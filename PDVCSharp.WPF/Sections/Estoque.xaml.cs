using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Estoque : UserControl, IScreenActivation
    {
        private readonly EstoqueViewModel _vm;

        public Estoque()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<EstoqueViewModel>();
            DataContext = _vm;
        }

        public void OnNavigatedTo() => _ = _vm.CarregarAsync();

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
            => MainWindow.Navigation.Navigate(AppScreen.CaixaLivre);
    }
}
