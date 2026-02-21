using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.WPF.Contexts
{
    // Master é uma classe estática que armazena o ESTADO GLOBAL da aplicação (sessão)
    // 💡 DICA: "static" = existe apenas uma instância compartilhada por todo o app.
    //    Qualquer tela pode acessar Master.Usuario, Master.Caixa, etc.
    public static class Master
    {
        public static SessaoUsuario Usuario { get; set; }  // Usuário logado (null = ninguém)
        public static SessaoCaixa Caixa { get; set; }      // Caixa aberto (null = fechado)
        public static SessaoVenda Venda { get; set; }      // Venda em andamento (null = sem venda)
    }
}
