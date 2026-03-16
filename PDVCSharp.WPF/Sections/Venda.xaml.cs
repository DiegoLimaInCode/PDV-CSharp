using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;


namespace PDVCSharp.WPF.Sections {
    // Tela principal de Venda — operador adiciona/remove produtos e finaliza a venda.
    // INotifyPropertyChanged = permite atualizar a tela via Data Binding.
    public partial class Venda : UserControl, INotifyPropertyChanged {
        private readonly IProductRepository _productRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly VendaService _vendaService;

        // ObservableCollection = lista que NOTIFICA a tela quando itens são adicionados/removidos
        private ObservableCollection<ProdutoVenda> _produtos;

        public ObservableCollection<ProdutoVenda> Produtos {
            get => _produtos;
            set {
                _produtos = value;
                OnPropertyChanged();     // Notifica a tela que a lista mudou
                AtualizarTotais();       // Recalcula subtotal/total
            }
        }

       

        public Venda(VendaService vendaService) : this() {
           

            _vendaService = vendaService;
        }

        public Venda() {
            InitializeComponent();

            _productRepository = App.ServiceProvider.GetRequiredService<IProductRepository>();
            _estoqueRepository = App.ServiceProvider.GetRequiredService<IEstoqueRepository>();
            _vendaService = App.ServiceProvider.GetRequiredService<VendaService>();

            Produtos = new ObservableCollection<ProdutoVenda>(); // Define a fonte de dados da lista na tela
            LstProdutos.ItemsSource = Produtos;
            // Quando a coleção mudar (add/remove), recalcula os totais
            Produtos.CollectionChanged += (s, e) => AtualizarTotais();

            CarregarProdutosDaBase();
        }

        private void CarregarProdutosDaBase()
        {
            var produtosBanco = _productRepository.GetAll();

            foreach (var produto in produtosBanco)
            {
                Produtos.Add(new ProdutoVenda
                {
                    Id = produto.Id,
                    Name = produto.Name,
                    Price = produto.Price,
                    Quantity = 1,
                    EstoqueDisponivel = produto.Quantity,
                    ImagePath = produto.ImagePath ?? string.Empty
                });
            }
        }


        public Produto? BuscarProduto(string codigo) {
            return null;
        }

        // Botão "−" — diminui a quantidade do produto
        // 💡 DICA: button.Tag contém o produto associado (definido no XAML via Tag="{Binding}")
        private void BtnDiminuir_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is ProdutoVenda produto) {
                if (produto.Quantity > 1) {
                    produto.Quantity -= 1;
                    AtualizarTotais();
                }
                else {
                    var result = MessageBox.Show(
                        $"Deseja remover '{produto.Name}' da venda?",
                        "Remover Produto",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes) {
                        Produtos.Remove(produto);
                    }
                }
            }
        }

        // Botão "+" — aumenta a quantidade do produto
        private void BtnAumentar_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is ProdutoVenda produto) {
                produto.Quantity += 1;
                AtualizarTotais();
            }
        }

        // Botão "Remover" — remove o produto da venda (com confirmação)
        private void BtnRemover_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Tag is ProdutoVenda produto) {
                var result = MessageBox.Show(
                    $"Deseja remover '{produto.Name}' da venda?",
                    "Remover Produto",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    Produtos.Remove(produto);
                }
            }
        }

        // Recalcula e atualiza subtotal, desconto e total na tela
        private void AtualizarTotais() {
            // .Sum() soma os totais de todos os produtos (preço × quantidade)
            decimal subtotal = Produtos.Sum(p => p.Total);
            decimal desconto = 0; // Desconto fixo em 0 por enquanto

            // "F2" = formato com 2 casas decimais (ex: "15.50")
            TxtSubtotal.Text = $"R$ {subtotal:F2}";
            TxtDesconto.Text = $"R$ {desconto:F2}";
            TxtTotal.Text = $"R$ {(subtotal - desconto):F2}";
        }

        // Botão "Finalizar Venda" — valida estoque, debita e navega para VendaFinal
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Produtos.Any())
            {
                MessageBox.Show("Adicione produtos à venda antes de finalizar.",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var itensVendidos = Produtos
                .Select(p => new ProdutoVendido(p.Name, p.Quantity))
                .ToList();

            bool estoqueOk = await _productRepository.ValidarEstoque(itensVendidos);

            if (!estoqueOk)
            {
                var produtosSemEstoque = Produtos
                    .Where(p => p.Quantity > GetQuantidadeBanco(p.Name))
                    .Select(p => p.Name)
                    .ToList();

                MessageBox.Show(
                    $"Estoque insuficiente para: {string.Join(", ", produtosSemEstoque)}\n\n" +
                    "Reduza a quantidade ou remova esses produtos da venda.",
                    "Estoque insuficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                foreach (var item in Produtos)
                {
                    await _estoqueRepository.RegistrarSaida(
                        produtoId: item.Id,
                        quantidade: item.Quantity,
                        motivo: "Venda"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao atualizar estoque: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var telaVendaFinal = new VendaFinal(Produtos);
            var containerPai = this.Parent as Panel;

            if (containerPai != null) {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaVendaFinal);
            }
        }

        private double GetQuantidadeBanco(string nomeProduto)
        {
            return _productRepository.GetAll()
                .Where(p => p.Name == nomeProduto)
                .Select(p => p.Quantity)
                .FirstOrDefault();
        }

        // Botão "Cancelar Venda" — volta para a tela de Caixa Livre
        private void Button_Click_2(object sender, RoutedEventArgs e) {
            if (Produtos.Any()) {
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
            if (telaCaixaLivre != null) {
                telaCaixaLivre.Visibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private void TextBox_GotFocus(object sender, RoutedEventArgs e) {
            var textBox = sender as TextBox;

            if (textBox != null) {
                textBox.BorderBrush = System.Windows.Media.Brushes.Gray;

            }

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
            var textBox = sender as TextBox;

            if (textBox != null) {
                textBox.BorderBrush = System.Windows.Media.Brushes.LightGray;

            }
        }

        private async void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;

            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (string.IsNullOrWhiteSpace(textBox.Text))
                return;

            if (!Guid.TryParse(textBox.Text, out Guid result)) {
                MessageBox.Show("Id inválido.");
                return;
            }

            Produto? produto;
            try {
                produto = await _vendaService.GetProductById(result);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (produto == null) {
                MessageBox.Show("Produto não encontrado.");
                return;
            }

            var produtoExistente = Produtos.FirstOrDefault(p => p.Name == produto.Name);

            if (produtoExistente != null) {
                produtoExistente.Quantity += 1;
            }
            else {
                var novoProdutoVenda = new ProdutoVenda {
                    Name = produto.Name,
                    Price = produto.Price,
                    Quantity = 1,
                    ImagePath = produto.ImagePath
                };
                Produtos.Add(novoProdutoVenda);
            }

            textBox.Text = "";
            textBox.Focus();
        }
    }

    // ProdutoVenda = versão do Produto usada na tela de venda (com quantidade editável)
    // Implementa INotifyPropertyChanged para que a tela atualize ao mudar preço/quantidade
    // 💡 DICA: Separada de Produto (Domain) porque a tela precisa de funcionalidades extras.
    public class ProdutoVenda : INotifyPropertyChanged {
        // Campos privados (backing fields) — armazenam o valor real
        private Guid _id;
        private string _name = string.Empty;
        private decimal _price;
        private double _quantity;
        private string _imagePath = string.Empty;
        private double _estoqueDisponivel;

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged();
            }
        }

        public double EstoqueDisponivel {
            get => _estoqueDisponivel;
            set {
                _estoqueDisponivel = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath {
            get => _imagePath;
            set {
                _imagePath = value;
                OnPropertyChanged();
            }
        }
        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Price {
            get => _price;
            set {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public double Quantity {
            get => _quantity;
            set {
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

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
