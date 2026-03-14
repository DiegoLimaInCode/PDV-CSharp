using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Produto>
    {
        Task BaixarEstoque(IEnumerable<ProdutoVendido> itensVendidos);
        Task<bool> ValidarEstoque(IEnumerable<ProdutoVendido> itensVendidos);
        public record ProdutoVendido(string Name, double QuantidadeVendida);


    }
}
