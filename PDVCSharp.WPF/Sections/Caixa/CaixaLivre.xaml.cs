using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections.Caixa
{
    public partial class CaixaLivre : UserControl, IScreenActivation
    {
        private readonly AberturaViewModel _vm;

        public CaixaLivre()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<AberturaViewModel>();
            DataContext = _vm;
        }

        public void OnNavigatedTo() => _vm.Refresh();

        private void BtnIniciarVenda_Click(object sender, RoutedEventArgs e)
        {
            Master.Venda = new SessaoVenda();
            MainWindow.Navigation.Navigate(AppScreen.Venda);
        }

        private void BtnEstoque_Click(object sender, RoutedEventArgs e)
            => MainWindow.Navigation.Navigate(AppScreen.Estoque);

        private void BtnHistorico_Click(object sender, RoutedEventArgs e)
            => MainWindow.Navigation.Navigate(AppScreen.Historico);

        private void BtnFechamento_Click(object sender, RoutedEventArgs e)
            => MainWindow.Navigation.Navigate(AppScreen.Fechamento);
    }
}
