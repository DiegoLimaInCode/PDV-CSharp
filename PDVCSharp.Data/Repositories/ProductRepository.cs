using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    public class ProductRepository : Repository<Produto>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Produto?> GetBySkuOrName(string termo)
        {
            var busca = termo.Trim();
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .FirstOrDefaultAsync(p =>
                    p.Sku.ToLower() == busca.ToLower() ||
                    p.Name.ToLower().Contains(busca.ToLower()));
        }

        public async Task<bool> ValidarEstoque(IEnumerable<ProdutoVendido> itensVendidos)
        {
            foreach (var item in itensVendidos)
            {
                var produtoBanco = await BuscarProduto(item);
                if (produtoBanco == null || produtoBanco.Quantity < item.QuantidadeVendida)
                    return false;
            }
            return true;
        }

        public async Task BaixarEstoque(IEnumerable<ProdutoVendido> itensVendidos, bool commit = true)
        {
            foreach (var item in itensVendidos)
            {
                var produtoBanco = await BuscarProduto(item, tracking: true)
                    ?? throw new DomainException($"Produto '{item.Name}' não encontrado no banco ao baixar estoque.");

                if (produtoBanco.Quantity < item.QuantidadeVendida)
                {
                    throw new EstoqueInsuficienteException(
                        $"Estoque insuficiente para '{produtoBanco.Name}'. Disponível: {produtoBanco.Quantity}, solicitado: {item.QuantidadeVendida}.");
                }

                var antes = produtoBanco.Quantity;
                produtoBanco.Quantity -= item.QuantidadeVendida;
                produtoBanco.UpdatedAt = DateTime.UtcNow;

                _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    ProdutoId = produtoBanco.Id,
                    ProdutoNome = produtoBanco.Name,
                    QuantidadeAntes = antes,
                    QuantidadeMovida = item.QuantidadeVendida,
                    QuantidadeDepois = produtoBanco.Quantity,
                    Tipo = TipoMovimentacao.Saida,
                    Motivo = "Venda"
                });
            }

            if (commit)
            {
                await Commit();
            }
        }

        private async Task<Produto?> BuscarProduto(ProdutoVendido item, bool tracking = false)
        {
            var query = tracking ? _dbSet.AsQueryable() : _dbSet.AsNoTracking();
            query = query.Where(p => !p.IsDeleted);

            if (item.ProdutoId != Guid.Empty)
            {
                return await query.FirstOrDefaultAsync(p => p.Id == item.ProdutoId);
            }

            return await query.FirstOrDefaultAsync(p => p.Name == item.Name);
        }
    }
}
