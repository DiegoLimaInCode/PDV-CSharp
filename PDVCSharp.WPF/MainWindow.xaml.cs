using PDVCSharp.Domain.Entities;
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

namespace PDVCSharp.WPF
{
    /// <summary>
    /// MainWindow é a janela principal do aplicativo.
    /// Ela contém o "MainContainer" (Grid) onde as telas (Login, Abertura, Venda, etc.)
    /// são exibidas/escondidas conforme o fluxo do PDV.
    ///
    /// 💡 DICA: "partial" significa que esta classe é dividida em dois arquivos:
    ///    - MainWindow.xaml = define a APARÊNCIA (XAML/HTML-like)
    ///    - MainWindow.xaml.cs = define o COMPORTAMENTO (C# code-behind)
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Carrega e renderiza os componentes visuais definidos no XAML
            InitializeComponent();
        }

    }
}