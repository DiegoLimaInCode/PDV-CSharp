using PDVCSharp.Data.Repositories;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Application.Services {
 public class VendaService  {
        private readonly IProductRepository _productRepository;

        public VendaService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        
       public async Task<Produto> GetProductById(Guid id) {
           var checkIfProductExists= await _productRepository.GetById(id);

            if (checkIfProductExists == null) {
                throw new ArgumentNullException("Não existe um produto com esse Id");
            }

           
            return checkIfProductExists;
            
            
        }

    }
}
