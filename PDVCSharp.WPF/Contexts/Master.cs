using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDVCSharp.WPF.Contexts
{
    // Master é uma classe estática que armazena o ESTADO GLOBAL da aplicação (sessão)
    // 💡 DICA: "static" = existe apenas uma instância compartilhada por todo o app.
    //    Qualquer tela pode acessar Master.Usuario, Master.Caixa, etc.
    //
    // ⚠️ DICA AVANÇADA: Em apps maiores, evite estado global estático.
    //    Prefira usar injeção de dependência com serviços Singleton ou Scoped.
    public static class Master
    {
        // Sessão do usuário logado (nome do operador)
        // Se for null, nenhum usuário está logado
        public static SessaoUsuario Usuario { get; set; }

        // Sessão do caixa (valor de abertura, etc.)
        // Se for null, o caixa ainda não foi aberto
        public static SessaoCaixa Caixa { get; set; }

        // Sessão da venda atual (carrinho de produtos)
        // Se for null, não há venda em andamento
        public static SessaoVenda Venda { get; set; }
    }
}
