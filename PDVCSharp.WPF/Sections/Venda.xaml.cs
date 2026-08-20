using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDVCSharp.WPF.Sections
{
    public partial class Venda : UserControl, IScreenActivation
    {
        private readonly VendaViewModel _vm;

        public void LimparCarrinho() => _vm.LimparCarrinho();

        public Venda()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<VendaViewModel>();
            DataContext = _vm;
        }

        public void OnNavigatedTo() => _ = _vm.CarregarCatalogoAsync();

        private async void TxtBusca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await _vm.CarregarCatalogoAsync();
                if (_vm.Catalogo.Count == 1)
                {
                    _vm.AdicionarAoCarrinho(_vm.Catalogo[0]);
                    _vm.Busca = string.Empty;
                    await _vm.CarregarCatalogoAsync();
                }
            }
            else if (e.Key == Key.Escape)
            {
                BtnCancelar_Click(sender, e);
            }
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.Carrinho.Any())
            {
                MessageBox.Show("Adicione produtos à venda antes de finalizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainWindow.Navigation.GetScreen<VendaFinal>()?.DefinirProdutos(_vm.Carrinho);
            MainWindow.Navigation.Navigate(AppScreen.VendaFinal);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Carrinho.Any() &&
                MessageBox.Show("Deseja realmente cancelar a venda atual?", "Cancelar Venda",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            _vm.LimparCarrinho();
            Master.Venda = null;
            MainWindow.Navigation.Navigate(AppScreen.CaixaLivre);
        }
    }
}
