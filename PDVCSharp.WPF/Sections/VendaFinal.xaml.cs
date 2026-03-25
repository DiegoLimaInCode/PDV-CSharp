using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Contexts;
using PDVCSharp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDVCSharp.WPF.Sections {

    public partial class VendaFinal : UserControl {

        private ObservableCollection<ProdutoVenda> _produtos;
        private readonly VendaFinalService _finalService;

        private decimal _subtotal = 0;
        private decimal _desconto = 0;
        private decimal _totalVenda = 0;

        public ObservableCollection<PagamentoCartao> PagamentosCartao { get; set; }
        = new ObservableCollection<PagamentoCartao>();


        public ObservableCollection<ProdutoVenda> Produtos {
            get => _produtos;
            set {
                _produtos = value;
            }
        }

        public VendaFinal() {
            InitializeComponent();
            _finalService = App.ServiceProvider.GetRequiredService<VendaFinalService>();
            _produtos = new ObservableCollection<ProdutoVenda>();
            DgCartoes.ItemsSource = PagamentosCartao;

            Loaded += (_, __) => {
                AtualizarEstadoBotaoAdicionarPagamento();
                AtualizarEstadoTotalRecebido();
            };

            AtualizarEstadoBotaoAdicionarPagamento();
            AtualizarEstadoTotalRecebido();
        }

        public void DefinirProdutos(ObservableCollection<ProdutoVenda> produtos) {
            _produtos = produtos;
            PagamentosCartao.Clear();
            CmbCliente.SelectedIndex = 0;
            CmbFormaPagamento.SelectedIndex = 0;
            TxtTotalRecebido.Text = "0,00";
            RecalcularTotais();
            AtualizarEstadoBotaoAdicionarPagamento();
        }

        private bool FormaPagamentoCartaoSelecionada(out string tipoCartao) {
            tipoCartao = string.Empty;

            if (CmbFormaPagamento == null) {
                return false;
            }

            if (CmbFormaPagamento.SelectedIndex == 1) {
                tipoCartao = "Crédito";
                return true;
            }

            if (CmbFormaPagamento.SelectedIndex == 2) {
                tipoCartao = "Débito";
                return true;
            }

            return false;
        }

        private void AtualizarEstadoBotaoAdicionarPagamento() {
            if (BtnAdicionarPagamento == null) {
                return;
            }

            bool habilitado = FormaPagamentoCartaoSelecionada(out _);
            BtnAdicionarPagamento.IsEnabled = habilitado;
            BtnAdicionarPagamento.Opacity = habilitado ? 1.0 : 0.6;
        }

        private void CarregarDadosTela() {
            RecalcularTotais();
        }

        public void RecalcularTotais() {
            if (Produtos is null) {
                return;
            }

            var clienteVip = CmbCliente.SelectedIndex == 1;

            _subtotal = 0;

            foreach (var produto in Produtos) {
                _subtotal += produto.Price * (decimal)produto.Quantity;
            }

            if (clienteVip) {
                _desconto = _subtotal / 2;
            }
            else {
                _desconto = 0;
            }

            TxtDesconto.Text = _desconto.ToString("F2");

            _totalVenda = 0;

            if (clienteVip) {
                _totalVenda = _subtotal - _desconto;
            }
            else {
                foreach (var produto in Produtos) {
                    _totalVenda += produto.Price * (decimal)produto.Quantity;
                }
            }

            TxtSubtotal.Text = _subtotal.ToString("F2");

            TxtTotal.Text = _totalVenda.ToString("F2");

            decimal totalRecebido = 0;

            if (FormaPagamentoCartaoSelecionada(out _)) {
                totalRecebido = _totalVenda;
            }
            else {
                string textoRecebido = TxtTotalRecebido.Text.Replace(".", ",");
                bool converteu = decimal.TryParse(
                    textoRecebido,
                    NumberStyles.Any,
                    new CultureInfo("pt-BR"),
                    out totalRecebido);

                if (!converteu || totalRecebido <= 0) {
                    totalRecebido = 0;
                }
            }

            decimal troco = totalRecebido - _totalVenda;
            if (troco < 0) {
                troco = 0;
            }
            TxtTroco.Text = troco.ToString("F2");

            decimal saldo = _totalVenda - totalRecebido;
            if (saldo < 0) {
                saldo = 0;
            }
            TxtSaldo.Text = saldo.ToString("F2");
        }

        public void TxtTotalRecebido_TextChanged(object sender, TextChangedEventArgs e) {
            if (_produtos != null) {
                RecalcularTotais();
            }
        }

        public void CmbCliente_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_produtos != null) {
                RecalcularTotais();
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e) {
            RecalcularTotais();
        }

        public void ValorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9,.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void ValorTextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Space) {
                e.Handled = true;
            }
        }

        public void CmbFormaPagamento_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            AtualizarEstadoBotaoAdicionarPagamento();
            AtualizarEstadoTotalRecebido();
            RecalcularTotais();
        }

        public void BtnAdicionarPagamento_Click(object sender, RoutedEventArgs e) {
            if (!FormaPagamentoCartaoSelecionada(out string tipoCartao)) {
                return;
            }

            var janela = new PagamentoCartaoWindow(_totalVenda, tipoCartao);

            if (janela.ShowDialog() == true) {
                PagamentosCartao.Add(janela.pagamentoCartao);
            }
        }

        public async void BtnFinalizar_Click(object sender, RoutedEventArgs e) {

            if (_produtos is null || !_produtos.Any()) {
                MessageBox.Show("Nenhum produto para finalizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string textoRecebido = TxtTotalRecebido.Text;
            textoRecebido = textoRecebido.Replace(".", ",");

            decimal totalRecebido = 0;
            bool converteu = decimal.TryParse(
                textoRecebido,
                NumberStyles.Any,
                new CultureInfo("pt-BR"),
                out totalRecebido);

            if (!converteu || totalRecebido <= 0) {
                MessageBox.Show("Informe um valor válido no campo Total Recebido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int indiceSelecionado = CmbFormaPagamento.SelectedIndex;
            FormaPagamento formaPagamento;

            if (indiceSelecionado == 0)
                formaPagamento = FormaPagamento.Caixa;
            else if (indiceSelecionado == 1)
                formaPagamento = FormaPagamento.Credito;
            else if (indiceSelecionado == 2)
                formaPagamento = FormaPagamento.Debito;
            else if (indiceSelecionado == 3)
                formaPagamento = FormaPagamento.Dinheiro;
            else
                formaPagamento = FormaPagamento.Cheque;

            TipoCliente tipoCliente = TipoCliente.Comum;
            if (CmbCliente.SelectedIndex == 1) {
                tipoCliente = TipoCliente.Premium;
            }

            List<ItemVenda> itensVenda = new List<ItemVenda>();

            foreach (var produto in _produtos) {
                var item = new ItemVenda();
                item.ProdutoId = produto.Id;
                item.Quantidade = (int)produto.Quantity;
                item.PrecoUnitario = produto.Price;

                itensVenda.Add(item);
            }

            try {
                await _finalService.FinalizarVenda(itensVenda, formaPagamento, tipoCliente, totalRecebido);
                MessageBox.Show("Venda finalizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                Master.Venda = null;
                this.Visibility = Visibility.Collapsed;
                var mainWindow = this.Parent as Grid;
                var telaFechamento = mainWindow?.Children.OfType<Fechamento>().FirstOrDefault();
                if (telaFechamento != null) {
                    telaFechamento.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            var itemSelecionado = DgCartoes.SelectedItem;

            if (itemSelecionado == null) {
                MessageBox.Show("Nenhum item selecionado");
                return;
            }

            PagamentosCartao.Remove((PagamentoCartao)itemSelecionado);
        }

        private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is DataGridRow row) {
                row.IsSelected = true;
                row.Focus();
            }
        }

        private void AtualizarEstadoTotalRecebido() {
            if (TxtTotalRecebido == null) {
                return;
            }

            bool pagamentoCartao = FormaPagamentoCartaoSelecionada(out _);

            TxtTotalRecebido.IsReadOnly = pagamentoCartao;
            TxtTotalRecebido.Focusable = !pagamentoCartao;

            if (pagamentoCartao) {
                TxtTotalRecebido.Text = _totalVenda.ToString("F2");
            }
            else if (TxtTotalRecebido.Text == _totalVenda.ToString("F2")) {
                TxtTotalRecebido.Text = "0,00";
            }
        }
    }
    }
