using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly AppDbContext _context;

        public EstoqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task RegistrarEntrada(Guid produtoId, double quantidade, string motivo, bool commit = true)
            => Registrar(produtoId, quantidade, motivo, TipoMovimentacao.Entrada, commit);

        public Task RegistrarSaida(Guid produtoId, double quantidade, string motivo, bool commit = true)
            => Registrar(produtoId, quantidade, motivo, TipoMovimentacao.Saida, commit);

        private async Task Registrar(Guid produtoId, double quantidade, string motivo, TipoMovimentacao tipo, bool commit)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId && !p.IsDeleted)
                ?? throw new DomainException($"Produto não encontrado: {produtoId}");

            if (tipo == TipoMovimentacao.Saida && produto.Quantity < quantidade)
            {
                throw new EstoqueInsuficienteException(
                    $"Estoque insuficiente para '{produto.Name}'. Disponível: {produto.Quantity}, solicitado: {quantidade}.");
            }

            var antes = produto.Quantity;
            produto.Quantity = tipo == TipoMovimentacao.Entrada
                ? produto.Quantity + quantidade
                : produto.Quantity - quantidade;
            produto.UpdatedAt = DateTime.UtcNow;

            _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                ProdutoNome = produto.Name,
                QuantidadeAntes = antes,
                QuantidadeMovida = quantidade,
                QuantidadeDepois = produto.Quantity,
                Tipo = tipo,
                Motivo = motivo
            });

            if (commit)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterHistorico(Guid produtoId)
        {
            return await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoId == produtoId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterHistoricoGeral()
        {
            return await _context.MovimentacoesEstoque
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterProdutosEstoqueBaixo(double estoqueMinimo)
        {
            return await _context.Produtos
                .Where(p => !p.IsDeleted && p.Quantity <= estoqueMinimo)
                .OrderBy(p => p.Quantity)
                .ToListAsync();
        }
    }
}
