using PDVCSharp.Domain.Entities;

namespace PDVCSharp.WPF.Contexts
{
    // Armazena os dados da venda em andamento
    // O Carrinho contém a lista de produtos que o cliente está comprando
    public class SessaoVenda
    {
        // Lista de produtos adicionados à venda
        // 💡 DICA: List<T> é uma coleção dinâmica (pode adicionar/remover itens)
        public List<Produto> Carrinho { get; set; }
    }
}
