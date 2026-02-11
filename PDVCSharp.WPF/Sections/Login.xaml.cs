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
            var usuario = TxtUsuario.Text.Trim();
            var senha = TxtPasswordVisible.Visibility == Visibility.Visible
                ? TxtPasswordVisible.Text
                : TxtPassword.Password;

            if (!PDVCSharp.WPF.AppSession.Operators.TryGetValue(usuario, out var senhaEsperada)
                || senha != senhaEsperada)
            {
                MessageBox.Show("Usuário ou senha inválidos.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PDVCSharp.WPF.AppSession.State.OperatorName = usuario;

            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {

                var telaLogin = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Login>().FirstOrDefault();
                var telaAbertura = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Abertura>().FirstOrDefault();

                if (telaLogin != null && telaAbertura != null)
                {
                    telaLogin.Visibility = Visibility.Collapsed; // Esconde o login
                    telaAbertura.Visibility = Visibility.Visible;    // Mostra a venda
                }
            }
        }

        //Metodo para esconder o texto de placeholder da senha 
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TxtPasswordVisible.Visibility != Visibility.Visible)
                TxtPasswordVisible.Text = TxtPassword.Password;

            UpdatePasswordPlaceholder();
        }

        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtPasswordVisible.Visibility == Visibility.Visible)
                TxtPassword.Password = TxtPasswordVisible.Text;

            UpdatePasswordPlaceholder();
        }

        private void TogglePassword_Checked(object sender, RoutedEventArgs e)
        {
            TxtPasswordVisible.Text = TxtPassword.Password;
            TxtPasswordVisible.Visibility = Visibility.Visible;
            TxtPassword.Visibility = Visibility.Collapsed;
            TxtPasswordVisible.Focus();
            TxtPasswordVisible.CaretIndex = TxtPasswordVisible.Text.Length;
            UpdatePasswordPlaceholder();
        }

        private void TogglePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtPassword.Password = TxtPasswordVisible.Text;
            TxtPassword.Visibility = Visibility.Visible;
            TxtPasswordVisible.Visibility = Visibility.Collapsed;
            TxtPassword.Focus();
            UpdatePasswordPlaceholder();
        }

        private void UpdatePasswordPlaceholder()
        {
            var valor = TxtPasswordVisible.Visibility == Visibility.Visible
                ? TxtPasswordVisible.Text
                : TxtPassword.Password;

            PlaceholderPassword.Visibility = string.IsNullOrEmpty(valor)
                ? Visibility.Visible
                : Visibility.Collapsed;
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
