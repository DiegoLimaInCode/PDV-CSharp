using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Produto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }
    }
}
