using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Domain.Interfaces {
  public interface IProductRepository : IRepository<Produto> {
        Task<Produto?> GetBySkuOrName(string termo);
        Task<bool> ValidarEstoque(IEnumerable<ProdutoVendido> itensVendidos);
        Task BaixarEstoque(IEnumerable<ProdutoVendido> itensVendidos, bool commit = true);
    }
}
