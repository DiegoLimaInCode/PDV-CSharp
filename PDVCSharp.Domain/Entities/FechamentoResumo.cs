using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.Domain.Entities
{
    public class FechamentoResumo
    {
        public decimal ValorAbertura { get; set; }
        public decimal TotalVendas { get; set; }
        public int QuantidadeVendas { get; set; }
        public decimal TotalDinheiro { get; set; }
        public decimal TotalCartaoCredito { get; set; }
        public decimal TotalCartaoDebito { get; set; }
        public decimal TotalCheque { get; set; }
        public decimal TotalCaixa { get; set; }
        public decimal SaldoFinal => ValorAbertura + TotalVendas;
    }
}
