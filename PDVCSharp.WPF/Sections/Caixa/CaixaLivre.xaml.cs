using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections.Caixa
{
    // Tela "Caixa Livre" — estado padrão quando o caixa está aberto mas sem venda ativa.
    // O operador pode iniciar uma nova venda a partir daqui.
    public partial class CaixaLivre : UserControl
    {
        public CaixaLivre()
        {
            InitializeComponent();
        }

        // Botão "Nova Venda" — inicia uma nova sessão de venda
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Cria uma nova sessão de venda (carrinho vazio)
            Master.Venda = new SessaoVenda();

            this.Visibility = Visibility.Collapsed; // Esconde Caixa Livre
            var mainWindow = this.Parent as Grid;
            if (mainWindow == null)
            {
                return;
            }

            var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();
            if (telaVenda != null)
            {
                telaVenda.Visibility = Visibility.Visible; // Mostra a tela de venda
            }
        }
    }
}
