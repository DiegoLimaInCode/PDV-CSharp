using System;

namespace PDVCSharp.Domain.Entities
{
    public class MovimentacaoEstoque : BaseEntity
    {
        public Guid ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public double QuantidadeAntes { get; set; }
        public double QuantidadeMovida { get; set; }
        public double QuantidadeDepois { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2
    }
}