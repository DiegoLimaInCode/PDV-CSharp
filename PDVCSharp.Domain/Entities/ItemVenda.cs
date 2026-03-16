using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PDVCSharp.Domain.Entities {
    public class ItemVenda : BaseEntity{

       
        public Venda Venda { get; set; } 
        public Produto Produto { get; set; }
        public Guid VendaId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantidade * PrecoUnitario;

    }
}
