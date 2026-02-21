using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

}
