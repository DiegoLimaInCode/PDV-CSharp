using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDVCSharp.WPF.Sections
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Venda : UserControl
    {

        public List<Produto>? Produtos { get; set; }

        public Venda() {
            InitializeComponent();

            var productsFile = File.ReadAllText("Produtos.json");
            Produtos = JsonSerializer.Deserialize<List<Produto>>(productsFile);
        }

        public Produto? BuscarProduto(string codigo) {
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var telaVendaFinal = new VendaFinal();
            var containerPai = this.Parent as Panel;

            if (containerPai != null) {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaVendaFinal);
            }

        }

     
        private void Button_Click_2(object sender, RoutedEventArgs e) {
            var telaAbertura = new Abertura();
            var containerPai = this.Parent as Panel;

            if(containerPai != null) {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaAbertura);
            }
        }
    }
}
