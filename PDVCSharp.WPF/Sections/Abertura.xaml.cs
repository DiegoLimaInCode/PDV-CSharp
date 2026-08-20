using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Abertura : UserControl, IScreenActivation
    {
        private readonly CaixaService _caixaService;
        private readonly UsuarioService _usuarioService;

        public Abertura()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<AberturaViewModel>();
            _caixaService = App.ServiceProvider.GetRequiredService<CaixaService>();
            _usuarioService = App.ServiceProvider.GetRequiredService<UsuarioService>();
        }

        public void OnNavigatedTo()
        {
            if (DataContext is AberturaViewModel vm)
            {
                vm.Refresh();
            }

            AtualizarAreaAdmin();
        }

        private void AtualizarAreaAdmin()
        {
            if (AdminOperatorArea is null)
            {
                return;
            }

            var isAdmin = Master.Usuario?.IsAdministrador == true;
            AdminOperatorArea.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            if (!isAdmin)
            {
                if (AdminOperatorPanel is not null) AdminOperatorPanel.Visibility = Visibility.Collapsed;
                if (DeleteOperatorPanel is not null) DeleteOperatorPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnToggleOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (AdminOperatorPanel is null) return;
            AdminOperatorPanel.Visibility = AdminOperatorPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void BtnCloseOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (AdminOperatorPanel is not null)
            {
                AdminOperatorPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void BtnToggleDeleteOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteOperatorPanel is null) return;
            var vaiMostrar = DeleteOperatorPanel.Visibility != Visibility.Visible;
            DeleteOperatorPanel.Visibility = vaiMostrar ? Visibility.Visible : Visibility.Collapsed;
            if (vaiMostrar)
            {
                await CarregarUsuariosParaExclusao();
            }
        }

        private async Task CarregarUsuariosParaExclusao()
        {
            try
            {
                var loginLogado = Master.Usuario?.OperatorName ?? string.Empty;
                var logins = await _usuarioService.ListarLoginsAsync(loginLogado);
                CmbUsersToDelete.ItemsSource = logins;
                CmbUsersToDelete.SelectedIndex = logins.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDeleteOperator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbUsersToDelete.SelectedItem is not string loginSelecionado || string.IsNullOrWhiteSpace(loginSelecionado))
                {
                    MessageBox.Show("Selecione um usuário para excluir.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Deseja realmente excluir o usuário '{loginSelecionado}'?", "Confirmar exclusão",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }

                await _usuarioService.ExcluirPorLoginAsync(loginSelecionado);
                MessageBox.Show("Usuário excluído com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await CarregarUsuariosParaExclusao();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSaveOperator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbOperatorCargo.SelectedItem is not ComboBoxItem cargoItem ||
                    !Enum.TryParse<Cargo>((cargoItem.Tag?.ToString() ?? string.Empty).Trim(), true, out var cargo))
                {
                    MessageBox.Show("Selecione o cargo do operador.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _usuarioService.CadastrarAsync(
                    TxtOperatorName.Text,
                    TxtOperatorLogin.Text,
                    PwdOperator.Password,
                    cargo);

                MessageBox.Show("Operador cadastrado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtOperatorName.Text = string.Empty;
                TxtOperatorLogin.Text = string.Empty;
                PwdOperator.Password = string.Empty;
                CmbOperatorCargo.SelectedIndex = 0;
                AdminOperatorPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var textoDigitado = (PlaceHolder_ValueBox.Text ?? string.Empty).Trim();
                decimal valorAbertura;
                if (string.IsNullOrWhiteSpace(textoDigitado) || textoDigitado == "R$ 200,00")
                {
                    valorAbertura = Master.Caixa?.ValorAbertura ?? 0m;
                }
                else
                {
                    var textoNormalizado = textoDigitado.Replace("R$", string.Empty).Trim();
                    if (!decimal.TryParse(textoNormalizado, NumberStyles.Number, new CultureInfo("pt-BR"), out valorAbertura))
                    {
                        throw new FormatException("Digite um valor válido no formato 200,00.");
                    }
                }

                var loginOperador = Master.Usuario?.OperatorName
                    ?? throw new InvalidOperationException("Nenhum usuário encontrado para registrar a abertura de caixa.");

                var sessao = await _caixaService.AbrirCaixaAsync(loginOperador, valorAbertura);
                Master.Caixa = new SessaoCaixa
                {
                    CaixaSessaoId = sessao.Id,
                    ValorAbertura = sessao.ValorAbertura
                };

                MainWindow.Navigation.Navigate(Master.Venda != null ? AppScreen.Venda : AppScreen.CaixaLivre);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Erro de Formato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlaceHolder_ValueBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtValorEntrada != null)
            {
                TxtValorEntrada.Text = PlaceHolder_ValueBox.Text;
            }
        }
    }
}
