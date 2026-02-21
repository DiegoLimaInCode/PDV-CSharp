using PDVCSharp.WPF.Contexts;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    // Tela de Abertura de Caixa — o operador informa o valor de troco inicial.
    // Após confirmar, navega para Caixa Livre ou Venda.
    public partial class Abertura : UserControl
    {
        public Abertura()
        {
            InitializeComponent();
        }

        // Quando o campo de valor recebe foco, limpa o placeholder
        // 💡 DICA: GotFocus é disparado quando o usuário clica no campo
        private void PlaceHolder_ValueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (PlaceHolder_ValueBox.Text == "R$ 200,00")
                PlaceHolder_ValueBox.Text = string.Empty;
        }

        // Quando o campo perde o foco, restaura o placeholder se vazio
        private void PlaceHolder_ValueBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (string.IsNullOrWhiteSpace(PlaceHolder_ValueBox.Text))
                PlaceHolder_ValueBox.Text = "R$ 200,00";
        }

        // Botão "Confirmar" — valida o valor e navega para a próxima tela
        private void BtnConfirmar_Click(Object sender, RoutedEventArgs e) {
            try {
                string textoDigitado = PlaceHolder_ValueBox.Text;

                // double.TryParse = tenta converter string para número
                // "out _" = descarta o resultado (só queremos saber se é válido)
                if (!string.IsNullOrEmpty(textoDigitado) && !double.TryParse(textoDigitado, out _)) {
                    throw new FormatException("Digite apenas números no valor de abertura.");
                }

                this.Visibility = Visibility.Collapsed; // Esconde a tela de abertura
                // 💡 DICA: "as Grid" é um cast seguro — retorna null se o Parent não for Grid
                var mainWindow = this.Parent as Grid;

                var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaVenda != null && telaCaixaLivre != null)
                {
                    // Navega conforme o estado do PDV
                    if (Master.Venda != null)
                    {
                        telaVenda.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        telaCaixaLivre.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (FormatException ex) {
                MessageBox.Show(ex.Message, "Erro de Formato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Sincroniza o texto digitado com o label de exibição do valor
        private void PlaceHolder_ValueBox_TextChanged(Object sender, TextChangedEventArgs e) {
            string textoDigitado = PlaceHolder_ValueBox.Text;

            if(TxtValorEntrada != null) {

                TxtValorEntrada.Text = textoDigitado;
            }
        }

    }
}

