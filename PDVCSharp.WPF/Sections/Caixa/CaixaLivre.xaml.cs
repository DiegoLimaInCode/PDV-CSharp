using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using System.Windows;
using System.Windows.Controls;

namespace PDVCSharp.WPF.Sections.Caixa
{
    /// <summary>
    /// Interaction logic for CaixaLivre.xaml
    /// </summary>
    public partial class CaixaLivre : UserControl
    {
        public CaixaLivre()
        {
            InitializeComponent();

            if (Master.Venda != null)
            {
                var telaVenda = new Venda();
                var containerPai = this.Parent as Panel;

                if (containerPai != null)
                {
                    containerPai.Children.Clear();
                    containerPai.Children.Add(telaVenda);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Master.Venda = new SessaoVenda();

            this.Visibility = Visibility.Collapsed;
            var mainWindow = this.Parent as Grid;

            var telaCaixaLivre = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
            var telaVenda = mainWindow.Children.OfType<PDVCSharp.WPF.Sections.Venda>().FirstOrDefault();
            telaVenda.Visibility = Visibility.Visible; // Mostra a tela de venda
        }
    }
}
