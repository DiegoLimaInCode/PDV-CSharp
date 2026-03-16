using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public Cargo Cargo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public enum Cargo
    {
        Caixa = 1,
        Administrador = 2,
    }
}
