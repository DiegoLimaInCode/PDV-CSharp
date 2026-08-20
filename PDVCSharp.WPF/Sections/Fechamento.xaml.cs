using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Fechamento : UserControl, IScreenActivation
    {
        private readonly FechamentoViewModel _vm;

        public Fechamento()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<FechamentoViewModel>();
            DataContext = _vm;
        }

        public void OnNavigatedTo() => _ = _vm.CarregarAsync();

        private async void BtnFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                    "Confirma o fechamento deste caixa? O operador voltará para a tela de abertura.",
                    "Fechar caixa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var fechado = await _vm.FecharCaixaAsync();
            if (!fechado)
            {
                MessageBox.Show("Não existe caixa aberto para fechar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Master.Caixa = null;
            Master.Venda = null;
            MessageBox.Show("Caixa fechado. Informe o valor de abertura para o próximo turno.", "Caixa encerrado", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow.Navigation.Navigate(AppScreen.Abertura);
        }

        private async void BtnFinalizarCompra_Click(object sender, RoutedEventArgs e)
        {
            var caixaAberto = await _vm.ExisteCaixaAbertoAsync();
            if (!caixaAberto)
            {
                MessageBox.Show("O caixa está fechado. Abra o caixa para continuar vendendo.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainWindow.Navigation.Navigate(AppScreen.Abertura);
                return;
            }

            MainWindow.Navigation.Navigate(AppScreen.CaixaLivre);
        }
    }
}
