using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.WPF.Contexts
{
    public static class Master
    {
        public static SessaoUsuario Usuario { get; set; }
        public static SessaoCaixa Caixa { get; set; }
        public static SessaoVenda Venda { get; set; }
    }
}
