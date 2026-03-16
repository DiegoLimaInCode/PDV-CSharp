using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Domain.Interfaces {
  public interface IProductRepository : IRepository<Produto> {
        Task<bool> ValidarEstoque(IEnumerable<ProdutoVendido> itensVendidos);
        Task BaixarEstoque(IEnumerable<ProdutoVendido> itensVendidos);
    }
}
