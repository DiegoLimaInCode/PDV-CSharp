using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDVCSharp.WPF.Sections
{
    // Tela de finalização da venda — operador informa o valor recebido e confirma o pagamento
    public partial class VendaFinal : UserControl
    {
        public VendaFinal()
        {
            InitializeComponent();
        }

        // Botão que reinicia a tela de finalização
        private void Button_Click(object sender, RoutedEventArgs e) {
            var telaVendaFinal = new VendaFinal();
            var containerPai = this.Parent as Panel;
            
            if(containerPai != null) {
                containerPai.Children.Clear();              // Limpa o container
                containerPai.Children.Add(telaVendaFinal);  // Recria a tela
            }

        }

        // Validação: permite apenas números, vírgula e ponto (para valores decimais)
        // 💡 DICA: PreviewTextInput é disparado ANTES do texto ser inserido.
        //    Se e.Handled = true, o texto NÃO é inserido (bloqueia a entrada).
        private void ValorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regex = expressão regular — "[^0-9,.]" = qualquer caractere que NÃO seja número/vírgula/ponto
            Regex regex = new Regex("[^0-9,.]");
            e.Handled = regex.IsMatch(e.Text); // Bloqueia se o caractere não é permitido
        }

        // Bloqueia a tecla espaço no campo de valor
        private void ValorTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Impede que o espaço seja digitado
            }
        }
    }
}