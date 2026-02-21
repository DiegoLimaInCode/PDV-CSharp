using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    // Produto herda de BaseEntity, ou seja, já possui Id, CreatedAt, UpdatedAt e IsDeleted
    // 💡 DICA: O " : BaseEntity" significa "herança" — Produto É UM BaseEntity com campos extras.
    public class Produto : BaseEntity
    {
        public string Name { get; set; }        // Nome do produto (ex: "Banana")
        public decimal Price { get; set; }       // Preço do produto — decimal é ideal para valores monetários
        public double Quantity { get; set; }     // Quantidade em estoque
        public string ImagePath { get; set; }    // Caminho da imagem do produto (ex: "Photos/banana.jpg")
        public string SegundoNome { get; set; }  // Nome alternativo/apelido do produto

        // Construtor: define o preço padrão como 0 ao criar um novo produto
        public Produto()
        {
            Price = 0;
        }
    }
}
