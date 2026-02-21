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
    // Tela principal de Venda — operador adiciona/remove produtos e finaliza a venda.
    // INotifyPropertyChanged = permite atualizar a tela via Data Binding.
    public partial class Venda : UserControl, INotifyPropertyChanged
    {
        // ObservableCollection = lista que NOTIFICA a tela quando itens são adicionados/removidos
        // 💡 DICA: Se usasse List<> normal, a tela não saberia que a lista mudou.
        private ObservableCollection<ProdutoVenda> _produtos;

        public ObservableCollection<ProdutoVenda> Produtos
        {
            get => _produtos;
            set
            {
                _produtos = value;
                OnPropertyChanged();     // Notifica a tela que a lista mudou
                AtualizarTotais();       // Recalcula subtotal/total
            }
        }

        public Venda()
        {
            InitializeComponent();

            Produtos = new ObservableCollection<ProdutoVenda>();

            CarregarProdutosExemplo(); // Carrega produtos do arquivo JSON

            LstProdutos.ItemsSource = Produtos; // Define a fonte de dados da lista na tela

            // Quando a coleção mudar (add/remove), recalcula os totais
            Produtos.CollectionChanged += (s, e) => AtualizarTotais();
        }

        // Carrega produtos do arquivo Produtos.json para a lista de venda
        private void CarregarProdutosExemplo()
        {
            try
            {
                if (File.Exists("Produtos.json")) // Verifica se o arquivo existe
                {
                    var productsFile = File.ReadAllText("Produtos.json"); // Lê todo o conteúdo
                    // Deserialize = converte JSON (texto) para objetos C#
                    var produtosBase = JsonSerializer.Deserialize<List<Produto>>(productsFile);

                    if (produtosBase != null && produtosBase.Any())
                    {
                        foreach (var produto in produtosBase)
                        {
                            Produtos.Add(new ProdutoVenda
                            {
                                Name = produto.Name,
                                Price = produto.Price,
                                Quantity = 1,
                                ImagePath = produto.ImagePath
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

        // Botão "−" — diminui a quantidade do produto
        // 💡 DICA: button.Tag contém o produto associado (definido no XAML via Tag="{Binding}")
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

        // Botão "+" — aumenta a quantidade do produto
        private void BtnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProdutoVenda produto)
            {
                produto.Quantity += 1;
                AtualizarTotais();
            }
        }

        // Botão "Remover" — remove o produto da venda (com confirmação)
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

        // Recalcula e atualiza subtotal, desconto e total na tela
        private void AtualizarTotais()
        {
            // .Sum() soma os totais de todos os produtos (preço × quantidade)
            decimal subtotal = Produtos.Sum(p => p.Total);
            decimal desconto = 0; // Desconto fixo em 0 por enquanto

            // "F2" = formato com 2 casas decimais (ex: "15.50")
            TxtSubtotal.Text = $"R$ {subtotal:F2}";
            TxtDesconto.Text = $"R$ {desconto:F2}";
            TxtTotal.Text = $"R$ {(subtotal - desconto):F2}";
        }

        // Botão "Finalizar Venda" — navega para a tela de finalização
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

        // Botão "Cancelar Venda" — volta para a tela de Caixa Livre
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

    // ProdutoVenda = versão do Produto usada na tela de venda (com quantidade editável)
    // Implementa INotifyPropertyChanged para que a tela atualize ao mudar preço/quantidade
    // 💡 DICA: Separada de Produto (Domain) porque a tela precisa de funcionalidades extras.
    public class ProdutoVenda : INotifyPropertyChanged
    {
        // Campos privados (backing fields) — armazenam o valor real
        private string _name = string.Empty;
        private decimal _price;
        private double _quantity;
        private string _imagePath = string.Empty;
         
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }
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

        // Propriedade calculada: Total = Preço × Quantidade
        // 💡 DICA: "=>" = propriedade somente-leitura calculada sob demanda.
        //    (decimal)Quantity = converte double para decimal (tipos diferentes)
        public decimal Total => Price * (decimal)Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
