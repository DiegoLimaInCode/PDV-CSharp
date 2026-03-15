using System;
using PDVCSharp.Domain.Entities;

namespace PDVCSharp.Domain.Entities
{
    public class MovimentoCaixa : BaseEntity
    {
        public DateTime DataHora { get; set; }

        public Guid CaixaSessaoId { get; set; }
        public CaixaSessao CaixaSessao { get; set; }

        public TipoMovimentoCaixa Tipo { get; set; }
        public OrigemMovimentoCaixa Origem { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }

    public enum TipoMovimentoCaixa
    {
        Entrada = 1,
        Saida = 2
    }

    public enum OrigemMovimentoCaixa
    {
        Abertura = 1,
        Venda = 2,
        Sangria = 3,
        Suprimento = 4,
        Ajuste = 5
    }
}
