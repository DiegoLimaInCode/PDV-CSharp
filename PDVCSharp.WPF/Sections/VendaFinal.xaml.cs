using Microsoft.Extensions.DependencyInjection;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDVCSharp.WPF.Sections {
    
    public partial class VendaFinal : UserControl {

        private ObservableCollection<ProdutoVenda> _produtos;
        private readonly VendaFinalService _finalService;

        private decimal _subtotal = 0;
        private decimal _desconto = 0;
        private decimal _totalVenda = 0;

        public ObservableCollection<ProdutoVenda> Produtos {
            get => _produtos;
            set {
                _produtos = value;
            }
        }

        public VendaFinal(VendaFinalService finalService) : this() {
            _finalService = finalService;
        }

        public VendaFinal(ObservableCollection<ProdutoVenda> produtos) : this() {
            _produtos = produtos;
            _finalService = App.ServiceProvider.GetRequiredService<VendaFinalService>();
            CarregarDadosTela();
        }

        public VendaFinal() {
            InitializeComponent();
        }

        private void CarregarDadosTela() {
            _subtotal = 0;
            foreach (var produto in _produtos) {
                _subtotal += produto.Price * (decimal)produto.Quantity;
            }
            TxtSubtotal.Text = _subtotal.ToString("F2");
            RecalcularTotais();
        }

        public void RecalcularTotais() {
            var clienteVip = CmbCliente.SelectedIndex == 1;

            if (clienteVip) {
                _desconto = _subtotal / 2;
            } else {
                _desconto = 0;
            }

            TxtDesconto.Text = _desconto.ToString("F2");

            _totalVenda = _subtotal - _desconto;
            TxtTotal.Text = _totalVenda.ToString("F2");

            string textoRecebido = TxtTotalRecebido.Text.Replace(".", ",");
            decimal totalRecebido = 0;
            decimal.TryParse(textoRecebido, out totalRecebido);

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
            var telaVendaFinal = new VendaFinal();
            var containerPai = this.Parent as Panel;

            if (containerPai != null) {
                containerPai.Children.Clear();
                containerPai.Children.Add(telaVendaFinal);
            }
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

        public async void BtnFinalizar_Click(object sender, RoutedEventArgs e) {
            string textoRecebido = TxtTotalRecebido.Text;
            textoRecebido = textoRecebido.Replace(".", ",");

            decimal totalRecebido = 0;
            bool converteu = decimal.TryParse(textoRecebido, out totalRecebido);    

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
                var venda = await _finalService.FinalizarVenda(itensVenda, formaPagamento, tipoCliente, totalRecebido);
                MessageBox.Show("Venda finalizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}