using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Data.Repositories {
   public class VendaRepository : Repository<Venda> , IVendaRepository {

        public VendaRepository(AppDbContext context) : base(context) {

        }


    }
}
