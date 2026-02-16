using PDVCSharp.WPF.Contexts;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    /// <summary>
    /// Interaction logic for Abertura.xaml
    /// </summary>
    public partial class Abertura : UserControl
    {
        public Abertura()
        {
            InitializeComponent();
        }

        private void PlaceHolder_ValueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (PlaceHolder_ValueBox.Text == "R$ 200,00")
                PlaceHolder_ValueBox.Text = string.Empty;
        }

        private void PlaceHolder_ValueBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (string.IsNullOrWhiteSpace(PlaceHolder_ValueBox.Text))
                PlaceHolder_ValueBox.Text = "R$ 200,00";
        }

        private void BtnConfirmar_Click(Object sender, RoutedEventArgs e) {
            try {
                string textoDigitado = PlaceHolder_ValueBox.Text;
              
                if (!string.IsNullOrEmpty(textoDigitado) && !double.TryParse(textoDigitado, out _)) {
                    throw new FormatException("Digite apenas números no valor de abertura.");
                }

                this.Visibility = Visibility.Collapsed;
                var mainWindow = this.Parent as Grid;

                var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaVenda != null && telaCaixaLivre != null)
                {
                    if (Master.Venda != null)
                    {
                        telaVenda.Visibility = Visibility.Visible; // Mostra a tela de venda
                    }
                    else
                    {
                        telaCaixaLivre.Visibility = Visibility.Visible; // Mostra a tela de caixa livre
                    }
                }
            }
            catch (FormatException ex) {
              
                MessageBox.Show(ex.Message, "Erro de Formato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PlaceHolder_ValueBox_TextChanged(Object sender, TextChangedEventArgs e) {
            string textoDigitado = PlaceHolder_ValueBox.Text;

            if(TxtValorEntrada != null) {
                
                TxtValorEntrada.Text = textoDigitado;
            }
        }

    }
}

