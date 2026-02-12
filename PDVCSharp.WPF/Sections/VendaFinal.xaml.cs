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
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class VendaFinal : UserControl
    {
        public VendaFinal()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var telaVendaFinal = new VendaFinal();
            var containerPai = this.Parent as Panel;

            if(containerPai != null) {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaVendaFinal);
            }

        }

        private void ValorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers, comma, and period for decimal input
            Regex regex = new Regex("[^0-9,.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ValorTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Allow navigation keys
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}