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
using System.Windows.Threading;

namespace PDVCSharp.WPF.Sections
{

    public partial class Login : UserControl
    {
        DispatcherTimer relogio = new DispatcherTimer();
        public Login()
        {
            InitializeComponent();

                relogio.Interval = TimeSpan.FromSeconds(1);
                relogio.Tick += Relogio_Tick;
                relogio.Start();
        }
        private void Relogio_Tick(object sender, EventArgs e)
        {
            // Atualiza o texto do relógio a cada segundo segundo horario do brasil
            TxtHora.Text = DateTime.Now.ToString("HH:mm");
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

        //Metodo para esconder o texto de placeholder da senha 
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (TxtPassword.Password.Length > 0)
            {
                PlaceholderPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlaceholderPassword.Visibility = Visibility.Visible;
            }
        }
        //Metodo para esconder o texto de placeholder do usuario 

        private void TxtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (TxtUsuario.Text.Length > 0)
            {
                PlaceholderUsuario.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlaceholderUsuario.Visibility = Visibility.Visible;
            }
        }
    }
}
