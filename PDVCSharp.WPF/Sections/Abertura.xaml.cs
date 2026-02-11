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
    }
}
