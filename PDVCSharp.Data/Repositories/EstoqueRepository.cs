using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
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

        public async Task RegistrarEntrada(Guid produtoId, double quantidade, string motivo)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId && !p.IsDeleted)
                ?? throw new Exception($"Produto não encontrado: {produtoId}");

            var antes = produto.Quantity;
            produto.Quantity += quantidade;
            produto.UpdatedAt = DateTime.UtcNow;
            _context.Produtos.Update(produto);

            _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                ProdutoNome = produto.Name,
                QuantidadeAntes = antes,
                QuantidadeMovida = quantidade,
                QuantidadeDepois = produto.Quantity,
                Tipo = TipoMovimentacao.Entrada,
                Motivo = motivo
            });

            await _context.SaveChangesAsync();
        }

        public async Task RegistrarSaida(Guid produtoId, double quantidade, string motivo)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId && !p.IsDeleted)
                ?? throw new Exception($"Produto não encontrado: {produtoId}");

            if (produto.Quantity < quantidade)
                throw new Exception($"Estoque insuficiente para '{produto.Name}'. " + $"Disponível: {produto.Quantity}, solicitado: {quantidade}.");

            var antes = produto.Quantity;
            produto.Quantity -= quantidade;
            produto.UpdatedAt = DateTime.UtcNow;
            _context.Produtos.Update(produto);

            _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                ProdutoNome = produto.Name,
                QuantidadeAntes = antes,
                QuantidadeMovida = quantidade,
                QuantidadeDepois = produto.Quantity,
                Tipo = TipoMovimentacao.Saida,
                Motivo = motivo
            });

            await _context.SaveChangesAsync();
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