using PDVCSharp.Domain.Entities;

namespace PDVCSharp.WPF.Contexts
{
    // Armazena os dados da venda em andamento
    public class SessaoVenda
    {
        // Lista de produtos que o cliente está comprando
        // 💡 DICA: List<T> é uma coleção dinâmica (pode adicionar/remover itens)
        public List<Produto> Carrinho { get; set; }
    }
}
