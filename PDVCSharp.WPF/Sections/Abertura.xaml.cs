using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections
{
    // Tela de Abertura de Caixa — o operador informa o valor de troco inicial.
    // Após confirmar, navega para Caixa Livre ou Venda.
    public partial class Abertura : UserControl
    {
        public Abertura()
        {
            InitializeComponent();
        }

        // Quando o campo de valor recebe foco, limpa o placeholder
        // 💡 DICA: GotFocus é disparado quando o usuário clica no campo
        private void PlaceHolder_ValueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (PlaceHolder_ValueBox.Text == "R$ 200,00")
                PlaceHolder_ValueBox.Text = string.Empty;
        }

        // Quando o campo perde o foco, restaura o placeholder se vazio
        private void PlaceHolder_ValueBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceHolder_ValueBox is null) return;

            if (string.IsNullOrWhiteSpace(PlaceHolder_ValueBox.Text))
                PlaceHolder_ValueBox.Text = "R$ 200,00";
        }

        // Botão "Confirmar" — valida o valor e navega para a próxima tela
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
                    Observacao = "Abertura de caixa"
                };

                context.MovimentosCaixa.Add(caixaMovimento);
                context.SaveChanges();

                Master.Caixa = new SessaoCaixa
                {
                    ValorAbertura = valorAbertura
                };

                this.Visibility = Visibility.Collapsed; // Esconde a tela de abertura
                // 💡 DICA: "as Grid" é um cast seguro — retorna null se o Parent não for Grid
                var mainWindow = this.Parent as Grid;
                if (mainWindow == null) return;

                var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
                var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();

                if (telaVenda != null && telaCaixaLivre != null)
                {
                    // Navega conforme o estado do PDV
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

        // Sincroniza o texto digitado com o label de exibição do valor
        private void PlaceHolder_ValueBox_TextChanged(Object sender, TextChangedEventArgs e) {
            string textoDigitado = PlaceHolder_ValueBox.Text;

            if(TxtValorEntrada != null) {

                TxtValorEntrada.Text = textoDigitado;
            }
        }

    }
}

