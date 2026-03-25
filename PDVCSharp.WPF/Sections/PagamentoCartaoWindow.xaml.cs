using PDVCSharp.Domain.Entities;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class PagamentoCartaoWindow : Window
    {
        public PagamentoCartao pagamentoCartao { get; private set; }

        public PagamentoCartaoWindow(decimal valor, string tipoCartaoSelecionado = "")
        {
            InitializeComponent();

            pagamentoCartao = new PagamentoCartao {
                Valor = valor
            };

            DataContext = pagamentoCartao;

            ConfigurarTipoCartao(tipoCartaoSelecionado);
            ConfigurarParcelas(valor);
        }

        private void ConfigurarTipoCartao(string tipoCartaoSelecionado)
        {
            TipoCartao.Items.Clear();

            if (tipoCartaoSelecionado == "Débito")
            {
                TipoCartao.Items.Add(new ComboBoxItem { Content = "Débito" });
                TipoCartao.SelectedIndex = 0;
                TipoCartao.IsEnabled = false;
                return;
            }

            if (tipoCartaoSelecionado == "Crédito")
            {
                TipoCartao.Items.Add(new ComboBoxItem { Content = "Crédito" });
                TipoCartao.SelectedIndex = 0;
                TipoCartao.IsEnabled = false;
                return;
            }

            TipoCartao.Items.Add(new ComboBoxItem { Content = "Crédito" });
            TipoCartao.Items.Add(new ComboBoxItem { Content = "Débito" });
            TipoCartao.SelectedIndex = 0;
            TipoCartao.IsEnabled = true;
        }

        private void ConfigurarParcelas(decimal valorTotal)
        {
            TipoParcela.Items.Clear();

            var cultura = new CultureInfo("pt-BR");

            for (int i = 1; i <= 12; i++)
            {
                decimal valorParcela = valorTotal / i;
                TipoParcela.Items.Add(new ComboBoxItem {
                    Content = $"{i}x / {valorParcela.ToString("F2", cultura)}"
                });
            }

            TipoParcela.SelectedIndex = 0;
        }

        public void AdicionarCartao() {
            pagamentoCartao = new PagamentoCartao {
                Valor = pagamentoCartao.Valor,
                Cartao = TipoCartao.Text,
                Parcela = TipoParcela.Text
            };
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            AdicionarCartao();
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
