using System;
using System.Collections.Generic;

namespace PDVCSharp.Domain.Entities
{
    public class CaixaSessao : BaseEntity
    {
        public DateTime DataHoraAbertura { get; set; }
        public decimal ValorAbertura { get; set; }
        public bool IsOpen { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    
        public ICollection<MovimentoCaixa> Movimentos { get; set; } = new List<MovimentoCaixa>();
    }
}
