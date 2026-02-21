using PDVCSharp.WPF.Contexts;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    /// <summary>
    /// Tela de Abertura de Caixa — o operador informa o valor de troco inicial.
    /// Após confirmar, o app navega para a tela de Caixa Livre ou Venda.
    /// </summary>
    public partial class Abertura : UserControl
    {
        public Abertura()
        {
            InitializeComponent();
        }

        // Quando o campo de valor recebe foco (clique), limpa o placeholder
        // 💡 DICA: GotFocus é disparado quando o usuário clica ou tabula para o campo
        private void PlaceHolder_ValueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            // Se o texto ainda é o placeholder, limpa para o usuário digitar
            if (PlaceHolder_ValueBox.Text == "R$ 200,00")
                PlaceHolder_ValueBox.Text = string.Empty;
        }

        // Quando o campo perde o foco (clique fora), restaura o placeholder se vazio
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

                // Valida se o texto digitado é um número válido
                // double.TryParse = tenta converter string para número, retorna true/false
                // "out _" = descarta o resultado (só queremos saber se é válido)
                if (!string.IsNullOrEmpty(textoDigitado) && !double.TryParse(textoDigitado, out _)) {
                    throw new FormatException("Digite apenas números no valor de abertura.");
                }

                // Esconde a tela de abertura
                this.Visibility = Visibility.Collapsed;

                // Obtém o Grid pai para acessar as outras telas
                // 💡 DICA: "as Grid" é um cast seguro — retorna null se o Parent não for Grid
                var mainWindow = this.Parent as Grid;

                // Busca as telas filhas dentro do Grid
                var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaVenda != null && telaCaixaLivre != null)
                {
                    // Navega conforme o estado: se há venda em andamento, vai para Venda; senão, Caixa Livre
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
                // Mostra erro de validação para o usuário
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

