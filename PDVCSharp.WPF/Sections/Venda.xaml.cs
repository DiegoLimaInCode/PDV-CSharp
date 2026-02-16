using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for Venda.xaml
    /// </summary>
    public partial class Venda : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<ProdutoVenda> _produtos;

        public ObservableCollection<ProdutoVenda> Produtos
        {
            get => _produtos;
            set
            {
                _produtos = value;
                OnPropertyChanged();
                AtualizarTotais();
            }
        }

        public Venda()
        {
            InitializeComponent();

            Produtos = new ObservableCollection<ProdutoVenda>();

            CarregarProdutosExemplo();

            LstProdutos.ItemsSource = Produtos;

            Produtos.CollectionChanged += (s, e) => AtualizarTotais();
        }

        private void CarregarProdutosExemplo()
        {
            try
            {
                if (File.Exists("Produtos.json"))
                {
                    var productsFile = File.ReadAllText("Produtos.json");
                    var produtosBase = JsonSerializer.Deserialize<List<Produto>>(productsFile);

                    if (produtosBase != null && produtosBase.Any())
                    {
                        foreach (var produto in produtosBase.Take(3))
                        {
                            Produtos.Add(new ProdutoVenda
                            {
                                Name = produto.Name,
                                Price = produto.Price,
                                Quantity = 1
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Produto? BuscarProduto(string codigo)
        {
            return null;
        }

        private void BtnDiminuir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProdutoVenda produto)
            {
                if (produto.Quantity > 1)
                {
                    produto.Quantity -= 1;
                    AtualizarTotais();
                }
                else
                {
                    var result = MessageBox.Show(
                        $"Deseja remover '{produto.Name}' da venda?",
                        "Remover Produto",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Produtos.Remove(produto);
                    }
                }
            }
        }

        private void BtnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProdutoVenda produto)
            {
                produto.Quantity += 1;
                AtualizarTotais();
            }
        }

        private void BtnRemover_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProdutoVenda produto)
            {
                var result = MessageBox.Show(
                    $"Deseja remover '{produto.Name}' da venda?",
                    "Remover Produto",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Produtos.Remove(produto);
                }
            }
        }

        private void AtualizarTotais()
        {
            decimal subtotal = Produtos.Sum(p => p.Total);
            decimal desconto = 0;

            TxtSubtotal.Text = $"R$ {subtotal:F2}";
            TxtDesconto.Text = $"R$ {desconto:F2}";
            TxtTotal.Text = $"R$ {(subtotal - desconto):F2}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Produtos.Any())
            {
                MessageBox.Show("Adicione produtos à venda antes de finalizar.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var telaVendaFinal = new VendaFinal();
            var containerPai = this.Parent as Panel;

            if (containerPai != null)
            {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaVendaFinal);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Produtos.Any())
            {
                var result = MessageBox.Show(
                    "Deseja realmente cancelar a venda atual?",
                    "Cancelar Venda",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }

            this.Visibility = Visibility.Collapsed;
            var mainWindow = this.Parent as Grid;

            var telaCaixaLivre = mainWindow?.Children.OfType<PDVCSharp.WPF.Sections.Caixa.CaixaLivre>().FirstOrDefault();
            if (telaCaixaLivre != null)
            {
                telaCaixaLivre.Visibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProdutoVenda : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _price;
        private double _quantity;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal Total => Price * (decimal)Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
