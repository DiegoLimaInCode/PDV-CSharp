using Microsoft.EntityFrameworkCore;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class EstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProductRepository _productRepository;

    public EstoqueService(IEstoqueRepository estoqueRepository, IProductRepository productRepository)
    {
        _estoqueRepository = estoqueRepository;
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<Produto>> ListarProdutosAsync()
        => Task.FromResult<IReadOnlyList<Produto>>(_productRepository.GetAll().OrderBy(p => p.Name).ToList());

    public Task<IEnumerable<MovimentacaoEstoque>> ObterHistoricoGeralAsync()
        => _estoqueRepository.ObterHistoricoGeral();

    public Task RegistrarEntradaAsync(Guid produtoId, double quantidade, string motivo)
        => _estoqueRepository.RegistrarEntrada(produtoId, quantidade, motivo);

    public Task RegistrarSaidaAsync(Guid produtoId, double quantidade, string motivo)
        => _estoqueRepository.RegistrarSaida(produtoId, quantidade, motivo);
}
