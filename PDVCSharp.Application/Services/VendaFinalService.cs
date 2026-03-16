using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Application.Services {
    public class VendaFinalService {

        private readonly IVendaRepository _vendaRepository;
        private readonly IProductRepository _productRepository;

        public VendaFinalService(IVendaRepository vendaRepository, IProductRepository productRepository) {
            _vendaRepository = vendaRepository;
            _productRepository = productRepository;
        }

        public async Task<Venda> FinalizarVenda(List<ItemVenda> itens, FormaPagamento formaPagamento, TipoCliente tipoCliente, decimal totalRecebido) {
            List<ItemVenda> _itens = itens;

            List<ProdutoVendido> produtosVendidos = new List<ProdutoVendido>();

            foreach (var item in itens) {
               var produtoEncontrado= await _productRepository.GetById(item.ProdutoId);
                var produtoVendido = new ProdutoVendido(produtoEncontrado.Name, item.Quantidade);
                produtosVendidos.Add(produtoVendido);
            }

            var estoqueValidado = await _productRepository.ValidarEstoque(produtosVendidos);

            if (!estoqueValidado) {
                throw new Exception("Estoque insuficiente");
            }

            decimal subtotal = 0;
            foreach (var item in itens) {
                subtotal += item.Subtotal;
            }

            var venda = new Venda();

            venda.Data = DateTime.Now;
            venda.FormaPagamento = formaPagamento;
            venda.TipoCliente = tipoCliente;
            venda.SubTotal = subtotal;
            venda.TotalRecebido = totalRecebido;
            venda.Itens = itens;

            venda.Calcular();

            if (totalRecebido < venda.Total) {
                throw new Exception("Pagamento insuficiente");
            }

            await _productRepository.BaixarEstoque(produtosVendidos);
            await _vendaRepository.Add(venda);

            return venda;


        }
    }
}