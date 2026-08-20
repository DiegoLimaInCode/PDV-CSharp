using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class VendaFinalService
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;

    public VendaFinalService(AppDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task<Venda> FinalizarVenda(
        List<ItemVenda> itens,
        FormaPagamento formaPagamento,
        TipoCliente tipoCliente,
        decimal totalRecebido,
        Guid? caixaSessaoId = null,
        string loginOperador = "")
    {
        if (itens is null || itens.Count == 0)
        {
            throw new DomainException("A venda precisa ter ao menos um item.");
        }

        var produtosVendidos = itens
            .Select(item => new ProdutoVendido(item.ProdutoId, string.Empty, item.Quantidade))
            .ToList();

        var estoqueValidado = await _productRepository.ValidarEstoque(produtosVendidos);
        if (!estoqueValidado)
        {
            throw new EstoqueInsuficienteException("Estoque insuficiente");
        }

        var subtotal = itens.Sum(item => item.Subtotal);
        var venda = new Venda
        {
            Data = DateTime.Now,
            FormaPagamento = formaPagamento,
            TipoCliente = tipoCliente,
            SubTotal = subtotal,
            TotalRecebido = totalRecebido,
            Itens = itens,
            CaixaSessaoId = caixaSessaoId
        };
        venda.Calcular();

        if (totalRecebido < venda.Total)
        {
            throw new PagamentoInsuficienteException("Pagamento insuficiente");
        }

        await using var transacao = await _context.Database.BeginTransactionAsync();

        await _productRepository.BaixarEstoque(produtosVendidos, commit: false);
        await _context.Vendas.AddAsync(venda);

        if (caixaSessaoId is Guid sessaoId && sessaoId != Guid.Empty)
        {
            _context.MovimentosCaixa.Add(new MovimentoCaixa
            {
                CaixaSessaoId = sessaoId,
                Tipo = TipoMovimentoCaixa.Entrada,
                Origem = OrigemMovimentoCaixa.Venda,
                Valor = venda.Total,
                DataHora = DateTime.Now,
                Observacao = $"Venda {venda.Id}",
                LoginOperador = loginOperador
            });
        }

        await _context.SaveChangesAsync();
        await transacao.CommitAsync();
        return venda;
    }
}
