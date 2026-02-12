using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PDVCSharp.WPF.Sections {
    /// <summary>
    /// Interaction logic for VendaFinal.xaml
    /// </summary>
    public partial class VendaFinal : UserControl {
        public VendaFinal() {
            InitializeComponent();
        }

        private void ValorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = true;

            if (!char.IsDigit(e.Text, 0))
                return;

            var textBox = (TextBox)sender;
            string digits = ExtrairDigitos(textBox.Text) + e.Text;
            textBox.Text = FormatarValor(digits);
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void ValorTextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Back || e.Key == Key.Delete) {
                e.Handled = true;

                var textBox = (TextBox)sender;
                string digits = ExtrairDigitos(textBox.Text);

                if (digits.Length > 0)
                    digits = digits[..^1];

                textBox.Text = FormatarValor(digits);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private static string ExtrairDigitos(string texto) {
            var sb = new StringBuilder();
            foreach (char c in texto) {
                if (char.IsDigit(c))
                    sb.Append(c);
            }
            return sb.ToString().TrimStart('0');
        }

        private static string FormatarValor(string digits) {
            if (string.IsNullOrEmpty(digits))
                digits = "0";

            long valor = long.Parse(digits);
            decimal valorDecimal = valor / 100m;
            return valorDecimal.ToString("N2", new CultureInfo("pt-BR"));
        }

      
    }
}