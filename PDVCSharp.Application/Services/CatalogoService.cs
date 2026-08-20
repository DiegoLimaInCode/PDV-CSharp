using Microsoft.EntityFrameworkCore;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class CatalogoService
{
    private readonly IProductRepository _productRepository;

    public CatalogoService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<Produto>> BuscarAsync(string? termo)
    {
        var produtos = await _productRepository.GetAll().OrderBy(p => p.Name).ToListAsync();
        if (string.IsNullOrWhiteSpace(termo))
        {
            return produtos;
        }

        var busca = termo.Trim();
        return produtos
            .Where(p =>
                p.Name.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                p.Categoria.Contains(busca, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
