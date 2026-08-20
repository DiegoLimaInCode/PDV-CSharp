using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class VendaService
{
    private readonly IProductRepository _productRepository;

    public VendaService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Produto> GetProductById(Guid id)
    {
        var produto = await _productRepository.GetById(id);
        if (produto is null)
        {
            throw new DomainException("Não existe um produto com esse Id");
        }

        return produto;
    }

    public Task<Produto?> BuscarProduto(string termo)
        => _productRepository.GetBySkuOrName(termo);

    public Task<IReadOnlyList<Produto>> ListarCatalogoAsync()
        => Task.FromResult<IReadOnlyList<Produto>>(_productRepository.GetAll().OrderBy(p => p.Name).ToList());
}
