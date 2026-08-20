using System.Windows;
using PDVCSharp.WPF.Navigation;
using PDVCSharp.WPF.Sections;
using PDVCSharp.WPF.Sections.Caixa;

namespace PDVCSharp.WPF
{
    public partial class MainWindow : Window
    {
        public static INavigationService Navigation { get; private set; } = null!;

        public MainWindow()
        {
            InitializeComponent();

            var screens = new Dictionary<AppScreen, System.Windows.Controls.UserControl>
            {
                [AppScreen.Login] = new Login(),
                [AppScreen.Abertura] = new Abertura(),
                [AppScreen.CaixaLivre] = new CaixaLivre(),
                [AppScreen.Venda] = new Venda(),
                [AppScreen.VendaFinal] = new VendaFinal(),
                [AppScreen.Fechamento] = new Fechamento(),
                [AppScreen.Estoque] = new Estoque(),
                [AppScreen.Historico] = new HistoricoVendas()
            };

            Navigation = new NavigationService(ScreenHost, screens);
            Navigation.Navigate(AppScreen.Login);
        }
    }
}
