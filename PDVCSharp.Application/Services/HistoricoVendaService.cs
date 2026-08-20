using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class HistoricoVendaService
{
    private readonly IVendaRepository _vendaRepository;

    public HistoricoVendaService(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public Task<IReadOnlyList<Venda>> ObterPorCaixaAsync(Guid caixaSessaoId)
        => _vendaRepository.ObterPorCaixaAsync(caixaSessaoId);
}
