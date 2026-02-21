using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using static System.Collections.Specialized.BitVector32;

namespace PDVCSharp.WPF.Sections
{
    // Tela de Login — é um UserControl (componente reutilizável dentro de uma janela)
    // 💡 DICA: UserControl ≠ Window. UserControl é uma "parte" de uma janela.
    public partial class Login : UserControl
    {
        // Serviço de autenticação — usado para validar login/senha no banco
        private readonly AuthService _authService;

        // Timer que atualiza o relógio na tela a cada segundo
        // 💡 DICA: DispatcherTimer roda na UI thread, então pode atualizar controles visuais.
        DispatcherTimer relogio = new DispatcherTimer();

        public Login()
        {
            InitializeComponent(); // Carrega os componentes visuais do XAML

            // Obtém o AuthService do contêiner de DI
            // GetRequiredService = retorna o serviço ou lança exceção se não encontrar
            _authService = App.ServiceProvider.GetRequiredService<AuthService>();

            // Configura o timer do relógio: dispara a cada 1 segundo
            relogio.Interval = TimeSpan.FromSeconds(1);
            relogio.Tick += Relogio_Tick; // Associa o evento Tick ao método
            relogio.Start();
        }

        // Evento chamado a cada segundo pelo timer — atualiza o horário na tela
        private void Relogio_Tick(object sender, EventArgs e)
        {
            TxtHora.Text = DateTime.Now.ToString("HH:mm"); // Formato 24h (ex: "14:30")
        }

        // Lógica principal de login — chamada quando o botão é clicado
        // "async Task" = método assíncrono (não trava a tela enquanto consulta o banco)
        private async Task LoginMethod(string usuario, string senha)
        {
            // Chama o serviço de autenticação (que consulta o banco via repositório)
            var success = await _authService.Login(usuario, senha);

            // Se login foi bem-sucedido, salva o nome do operador na sessão
            if (success)
            {
                Master.Usuario = new SessaoUsuario { OperatorName = usuario };
            }

            // Navega para a próxima tela conforme o estado do PDV
            // 💡 DICA: Window.GetWindow(this) obtém a janela pai deste UserControl
            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {
                // Busca todas as telas dentro do MainContainer (Grid da MainWindow)
                var telaLogin = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Login>().FirstOrDefault();
                var telaAbertura = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Abertura>().FirstOrDefault();
                var telaCaixaLivre = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.MainContainer.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaLogin != null && telaAbertura != null && telaCaixaLivre != null && telaVenda != null)
                {
                    telaLogin.Visibility = Visibility.Collapsed; // Esconde o login

                    // Decide qual tela mostrar baseado no estado do PDV:
                    if (Master.Caixa == null)
                    {
                        // Caixa não foi aberto → vai para tela de Abertura
                        telaAbertura.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (Master.Venda != null)
                        {
                            // Existe uma venda em andamento → vai para tela de Venda
                            telaVenda.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // Caixa aberto mas sem venda → vai para Caixa Livre
                            telaCaixaLivre.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        // Evento do botão de login
        // 💡 DICA: "async void" é o padrão correto APENAS para event handlers em WPF.
        //    NUNCA use .Wait() ou .Result em código async na UI thread — causa deadlock!
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var usuario = TxtUsuario.Text.Trim(); // .Trim() remove espaços extras

                // Pega a senha do campo visível ou do PasswordBox, conforme qual está ativo
                var senha = TxtPasswordVisible.Visibility == Visibility.Visible
                    ? TxtPasswordVisible.Text
                    : TxtPassword.Password;

                await LoginMethod(usuario, senha); // "await" = espera sem travar a tela
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao realizar login: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === Controle de placeholder e visibilidade da senha ===

        // Sincroniza o PasswordBox com o TextBox visível
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TxtPasswordVisible.Visibility != Visibility.Visible)
                TxtPasswordVisible.Text = TxtPassword.Password;

            UpdatePasswordPlaceholder();
        }

        // Sincroniza o TextBox visível com o PasswordBox
        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtPasswordVisible.Visibility == Visibility.Visible)
                TxtPassword.Password = TxtPasswordVisible.Text;

            UpdatePasswordPlaceholder();
        }

        // Toggle "mostrar senha" ativado → mostra TextBox, esconde PasswordBox
        // 💡 DICA: WPF não tem como mostrar/esconder senha no PasswordBox diretamente,
        //    por isso usamos dois controles (PasswordBox + TextBox) que se sincronizam.
        private void TogglePassword_Checked(object sender, RoutedEventArgs e)
        {
            TxtPasswordVisible.Text = TxtPassword.Password;
            TxtPasswordVisible.Visibility = Visibility.Visible;
            TxtPassword.Visibility = Visibility.Collapsed;
            TxtPasswordVisible.Focus();
            TxtPasswordVisible.CaretIndex = TxtPasswordVisible.Text.Length; // Cursor no final
            UpdatePasswordPlaceholder();
        }

        // Toggle "mostrar senha" desativado → esconde TextBox, mostra PasswordBox
        private void TogglePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtPassword.Password = TxtPasswordVisible.Text;
            TxtPassword.Visibility = Visibility.Visible;
            TxtPasswordVisible.Visibility = Visibility.Collapsed;
            TxtPassword.Focus();
            UpdatePasswordPlaceholder();
        }

        // Atualiza visibilidade do placeholder "Senha" conforme o campo está vazio ou não
        private void UpdatePasswordPlaceholder()
        {
            var valor = TxtPasswordVisible.Visibility == Visibility.Visible
                ? TxtPasswordVisible.Text
                : TxtPassword.Password;

            PlaceholderPassword.Visibility = string.IsNullOrEmpty(valor)
                ? Visibility.Visible   // Mostra placeholder se vazio
                : Visibility.Collapsed; // Esconde placeholder se há texto
        }

        // Atualiza visibilidade do placeholder "Usuário"
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
