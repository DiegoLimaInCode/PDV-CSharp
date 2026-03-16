using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using PDVCSharp.WPF.Contexts;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    public partial class Abertura : UserControl
    {
        public Abertura()
        {
            InitializeComponent();
            Loaded += Abertura_Loaded;
            IsVisibleChanged += Abertura_IsVisibleChanged;
        }

        private void Abertura_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarAreaAdmin();
        }

        private void Abertura_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AtualizarAreaAdmin();
            }
        }

        private void AtualizarAreaAdmin()
        {
            if (AdminOperatorArea is null)
            {
                return;
            }

            var isAdmin = string.Equals(Master.Usuario?.OperatorName, "admin", StringComparison.OrdinalIgnoreCase);
            AdminOperatorArea.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            if (!isAdmin)
            {
                if (AdminOperatorPanel is not null)
                {
                    AdminOperatorPanel.Visibility = Visibility.Collapsed;
                }

                if (DeleteOperatorPanel is not null)
                {
                    DeleteOperatorPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnToggleOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (AdminOperatorPanel is null)
            {
                return;
            }

            AdminOperatorPanel.Visibility = AdminOperatorPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void BtnCloseOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (AdminOperatorPanel is null)
            {
                return;
            }

            AdminOperatorPanel.Visibility = Visibility.Collapsed;
        }

        private async void BtnToggleDeleteOperatorPanel_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteOperatorPanel is null)
            {
                return;
            }

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
                using var scope = App.ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var loginLogado = Master.Usuario?.OperatorName ?? string.Empty;
                var logins = await context.Usuarios
                    .AsNoTracking()
                    .Where(u => !u.IsDeleted && u.Login.ToLower() != loginLogado.ToLower())
                    .OrderBy(u => u.Login)
                    .Select(u => u.Login)
                    .ToListAsync();

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

                var confirmacao = MessageBox.Show(
                    $"Deseja realmente excluir o usuário '{loginSelecionado}'?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmacao != MessageBoxResult.Yes)
                {
                    return;
                }

                using var scope = App.ServiceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var removido = await userRepository.DeleteByLoginHard(loginSelecionado);
                if (!removido)
                {
                    MessageBox.Show("Usuário não encontrado para exclusão.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    await CarregarUsuariosParaExclusao();
                    return;
                }

                MessageBox.Show("Usuário excluído com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await CarregarUsuariosParaExclusao();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSaveOperator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nome = TxtOperatorName.Text.Trim();
                var login = TxtOperatorLogin.Text.Trim();
                var senha = PwdOperator.Password.Trim();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    MessageBox.Show("Informe o nome do operador.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Informe o login do operador.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(senha))
                {
                    MessageBox.Show("Informe a senha do operador.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CmbOperatorCargo.SelectedItem is not ComboBoxItem cargoItem)
                {
                    MessageBox.Show("Selecione o cargo do operador.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var cargoTexto = (cargoItem.Tag?.ToString() ?? cargoItem.Content?.ToString() ?? string.Empty).Trim();
                if (!Enum.TryParse<Cargo>(cargoTexto, true, out var cargoSelecionado))
                {
                    MessageBox.Show("Cargo inválido.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var scope = App.ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var loginJaExiste = context.Usuarios.Any(u => u.Login.ToLower() == login.ToLower());
                if (loginJaExiste)
                {
                    MessageBox.Show("Já existe um usuário com esse login.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var novoUsuario = new Usuario
                {
                    Name = nome,
                    Login = login,
                    Password = senha,
                    Cargo = cargoSelecionado
                };

                context.Usuarios.Add(novoUsuario);
                context.SaveChanges();

                MessageBox.Show("Operador cadastrado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparFormularioOperador();
                AdminOperatorPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimparFormularioOperador()
        {
            TxtOperatorName.Text = string.Empty;
            TxtOperatorLogin.Text = string.Empty;
            PwdOperator.Password = string.Empty;
            CmbOperatorCargo.SelectedIndex = 0;
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
                var textoDigitado = (PlaceHolder_ValueBox.Text ?? string.Empty).Trim();
                decimal valorAbertura;

                if (string.IsNullOrWhiteSpace(textoDigitado) || textoDigitado == "R$ 200,00")
                {
                    valorAbertura = Master.Caixa?.ValorAbertura ?? 0m;
                }
                else
                {
                    var textoNormalizado = textoDigitado
                        .Replace("R$", string.Empty)
                        .Trim();

                    if (!decimal.TryParse(textoNormalizado, NumberStyles.Number, new CultureInfo("pt-BR"), out valorAbertura))
                    {
                        throw new FormatException("Digite um valor válido no formato 200,00.");
                    }
                }

                using var scope = App.ServiceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var loginOperador = Master.Usuario?.OperatorName;
                var usuario = !string.IsNullOrWhiteSpace(loginOperador)
                    ? context.Usuarios.FirstOrDefault(u => u.Login == loginOperador)
                    : context.Usuarios.FirstOrDefault();

                if (usuario is null)
                {
                    throw new InvalidOperationException("Nenhum usuário encontrado para registrar a abertura de caixa.");
                }

                var novaSessaoCaixa = new CaixaSessao
                {
                    ValorAbertura = valorAbertura,
                    DataHoraAbertura = DateTime.Now,
                    IsOpen = true,
                    UsuarioId = usuario.Id
                };

                context.CaixaSessoes.Add(novaSessaoCaixa);
                context.SaveChanges();

                var caixaMovimento = new MovimentoCaixa
                {
                    CaixaSessaoId = novaSessaoCaixa.Id,
                    Tipo = TipoMovimentoCaixa.Entrada,
                    Origem = OrigemMovimentoCaixa.Abertura,
                    Valor = valorAbertura,
                    DataHora = DateTime.Now,
                    Observacao = "Abertura de caixa",
                    LoginOperador = usuario.Login
                };

                context.MovimentosCaixa.Add(caixaMovimento);
                context.SaveChanges();

                Master.Caixa = new SessaoCaixa
                {
                    ValorAbertura = valorAbertura
                };

                this.Visibility = Visibility.Collapsed;
                var mainWindow = this.Parent as Grid;
                if (mainWindow == null) return;

                var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaVenda != null && telaCaixaLivre != null)
                {
                    if (Master.Venda != null)
                    {
                        telaVenda.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        telaCaixaLivre.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (FormatException ex) {
                MessageBox.Show(ex.Message, "Erro de Formato", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

