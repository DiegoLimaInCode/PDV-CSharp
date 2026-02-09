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
using static System.Collections.Specialized.BitVector32;
using System.Linq;

namespace PDVCSharp.WPF.Sections
{

    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {

                var telaLogin = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Login>().FirstOrDefault();
                var telaVenda = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaLogin != null && telaVenda != null)
                {
                    telaLogin.Visibility = Visibility.Collapsed; // Esconde o login
                    telaVenda.Visibility = Visibility.Visible;    // Mostra a venda
                }
            }
        }
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (TxtPassword.Password.Length > 0)
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
        }

        private void TxtUsuario_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
