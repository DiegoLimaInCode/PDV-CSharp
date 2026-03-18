using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Domain.Entities {
    public class PagamentoCartao {
        public string Cartao { get; set; }

        public string Parcela { get; set; }

        public decimal Valor { get; set; }

    }
}
