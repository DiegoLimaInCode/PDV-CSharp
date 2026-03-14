using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PDVCSharp.Data.Repositories
{
    public class ProductRepository : Repository<Produto>, IProductRepository
    {

        public ProductRepository(AppDbContext context) : base(context)
        {

        }


      
        public void CarregarProdutos() {
            try {
                if (File.Exists("Produtos.json")) // Verifica se o arquivo existe
                {
                    var productsFile = File.ReadAllText("Produtos.json"); // Lê todo o conteúdo
                    // Deserialize = converte JSON (texto) para objetos C#
                    var produtosBase = JsonSerializer.Deserialize<List<Produto>>(productsFile);

                    if (produtosBase != null && produtosBase.Any())
                    {
                        foreach (var produto in produtosBase)
                        {
                            _context.Add(new Produto
                            {
                                Name = produto.Name,
                                Price = produto.Price,
                                Quantity = 1,
                                ImagePath = produto.ImagePath
                            });
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar produtos do JSON", ex);
            }
        }

        public async Task<bool> ValidarEstoque(IEnumerable<IProductRepository.ProdutoVendido> itensVendidos)
        {
            foreach (var item in itensVendidos)
            {
                var produtoBanco = await _dbSet
                    .Where(p => p.Name == item.Name && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (produtoBanco == null || produtoBanco.Quantity < item.QuantidadeVendida)
                    return false;
            }
            return true;
        }
        public async Task BaixarEstoque(IEnumerable<IProductRepository.ProdutoVendido> itensVendidos)
        {
            foreach (var item in itensVendidos)
            {
                var produtoBanco = await _dbSet
                    .Where(p => p.Name == item.Name && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (produtoBanco == null)
                    throw new Exception($"Produto '{item.Name}' não encontrado no banco ao baixar estoque.");

                if (produtoBanco.Quantity < item.QuantidadeVendida)
                    throw new Exception($"Estoque insuficiente para '{item.Name}'. " + $"Disponível: {produtoBanco.Quantity}, solicitado: {item.QuantidadeVendida}.");

                produtoBanco.Quantity -= item.QuantidadeVendida;
                produtoBanco.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(produtoBanco);
            }
            await Commit();
        }

    }
}
