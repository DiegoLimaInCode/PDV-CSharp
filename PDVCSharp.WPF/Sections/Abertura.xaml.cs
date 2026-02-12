using System;
using System.Collections.Generic;
using System.Text;
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

              
                if (!double.TryParse(textoDigitado, out _)) {
                    throw new FormatException("Digite apenas números no valor de abertura.");
                }

                var telaVenda = new Venda();
                var containerPai = this.Parent as Panel;

                if (containerPai != null) {
                    containerPai.Children.Clear();
                    containerPai.Children.Add(telaVenda);
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

