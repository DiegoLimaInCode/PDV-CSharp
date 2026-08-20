using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    public class VendaRepository : Repository<Venda>, IVendaRepository
    {
        public VendaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Venda>> ObterPorCaixaAsync(Guid caixaSessaoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => !v.IsDeleted && v.CaixaSessaoId == caixaSessaoId)
                .OrderByDescending(v => v.Data)
                .ToListAsync();
        }
    }
}
