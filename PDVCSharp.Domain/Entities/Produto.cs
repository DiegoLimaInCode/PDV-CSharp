using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    // Produto herda de BaseEntity, ou seja, já possui Id, CreatedAt, UpdatedAt e IsDeleted
    // 💡 DICA: O " : BaseEntity" significa "herança" — Produto É UM BaseEntity com campos extras.
    public class Produto : BaseEntity
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }        // Nome do produto (ex: "Banana")
        [JsonPropertyName("price")]
        public decimal Price { get; set; }       // Preço — decimal é ideal para valores monetários
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }     // Quantidade em estoque
        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }    // Caminho da imagem (ex: "Photos/banana.jpg")

       

        // Construtor: define o preço padrão como 0 ao criar um novo produto
        public Produto()
        {
            Price = 0;
        }
    }
}
