using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Fechamento : UserControl
    {
        private readonly FechamentoViewModel _vm;

        public Fechamento()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetRequiredService<FechamentoViewModel>();
            DataContext = _vm;

            Loaded += async (_, _) => await _vm.CarregarAsync();
            IsVisibleChanged += async (_, _) =>
            {
                if (Visibility == Visibility.Visible)
                {
                    await _vm.CarregarAsync();
                }
            };
        }

        private async void BtnFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            var fechado = await _vm.FecharCaixaAsync();
            if (!fechado)
            {
                MessageBox.Show("Não existe caixa aberto para fechar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Caixa fechado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            MostrarTelaAbertura();
        }

        private async void BtnFinalizarCompra_Click(object sender, RoutedEventArgs e)
        {
            var caixaAberto = await _vm.ExisteCaixaAbertoAsync();
            if (!caixaAberto)
            {
                MessageBox.Show("O caixa está fechado. Abra o caixa para continuar vendendo.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                MostrarTelaAbertura();
                return;
            }

            MostrarTelaVenda();
        }

        private void MostrarTelaAbertura()
        {
            this.Visibility = Visibility.Collapsed;
            var mainWindow = this.Parent as Grid;

            var telaAbertura = mainWindow?.Children.OfType<Abertura>().FirstOrDefault();
            if (telaAbertura != null)
            {
                telaAbertura.Visibility = Visibility.Visible;
            }
        }

        private void MostrarTelaVenda()
        {
            this.Visibility = Visibility.Collapsed;
            var mainWindow = this.Parent as Grid;

            var telaVenda = mainWindow?.Children.OfType<Venda>().FirstOrDefault();
            if (telaVenda != null)
            {
                telaVenda.Visibility = Visibility.Visible;
            }
        }
    }
}