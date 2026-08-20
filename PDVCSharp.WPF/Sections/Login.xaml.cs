using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Login : UserControl
    {
        private readonly LoginViewModel _vm;

        public Login()
        {
            InitializeComponent();
            _vm = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            DataContext = _vm;
        }

        private async void Campo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await EntrarAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
            => await EntrarAsync();

        private async Task EntrarAsync()
        {
            var senha = TxtPasswordVisible.Visibility == Visibility.Visible
                ? TxtPasswordVisible.Text
                : TxtPassword.Password;
            await _vm.EntrarAsync(senha);
        }

        private void TogglePassword_Checked(object sender, RoutedEventArgs e)
        {
            TxtPasswordVisible.Text = TxtPassword.Password;
            TxtPasswordVisible.Visibility = Visibility.Visible;
            TxtPassword.Visibility = Visibility.Collapsed;
        }

        private void TogglePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtPassword.Password = TxtPasswordVisible.Text;
            TxtPassword.Visibility = Visibility.Visible;
            TxtPasswordVisible.Visibility = Visibility.Collapsed;
        }
    }
}
