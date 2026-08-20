using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Produto : BaseEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = "Geral";
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }
        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }

        public Produto()
        {
            Price = 0;
        }
    }
}
