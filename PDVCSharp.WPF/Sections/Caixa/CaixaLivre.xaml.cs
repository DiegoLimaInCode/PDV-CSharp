using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
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

            // Se já existe uma venda em andamento, redireciona para a tela de Venda
            if (Master.Venda != null)
            {
                var telaVenda = new Venda();
                var containerPai = this.Parent as Panel;

                if (containerPai != null)
                {
                    containerPai.Children.Clear();         // Remove todas as telas do container
                    containerPai.Children.Add(telaVenda);  // Adiciona a tela de venda
                }
            }
        }

        // Botão "Nova Venda" — inicia uma nova sessão de venda
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Cria uma nova sessão de venda (carrinho vazio)
            Master.Venda = new SessaoVenda();

            this.Visibility = Visibility.Collapsed; // Esconde Caixa Livre
            var mainWindow = this.Parent as Grid;

            var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
            var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();
            telaVenda.Visibility = Visibility.Visible; // Mostra a tela de venda
        }
    }
}
