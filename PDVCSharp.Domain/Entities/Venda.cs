using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PDVCSharp.Domain.Entities {


    public enum TipoCliente {
        Comum, 
        Premium,
    }

    public enum FormaPagamento {
        Credito, Debito, Dinheiro, Cheque, Caixa, Pix
    }

    public class Venda : BaseEntity {
        public DateTime Data { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DescontoAplicado { get; set; }
        public decimal Total { get; set; }
        public decimal TotalRecebido { get; set; }
        public TipoCliente TipoCliente { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public Guid? CaixaSessaoId { get; set; }
        public CaixaSessao? CaixaSessao { get; set; }
        public List<ItemVenda> Itens { get; set; } = new();

        [NotMapped]
        public List<PagamentoCartao> PagamentosCartaos { get; set; } = new();

        [NotMapped]
        public decimal Saldo => Total - TotalRecebido;

        public const decimal PercentualDescontoPremium = 0.10m;

        public decimal CalcularDesconto() {
            if (TipoCliente == TipoCliente.Premium) {
                return Math.Round(SubTotal * PercentualDescontoPremium, 2);
            }
            return 0;
        }

        public void Calcular() {
            DescontoAplicado = CalcularDesconto();
            Total = SubTotal - DescontoAplicado;
        }
    }
}
