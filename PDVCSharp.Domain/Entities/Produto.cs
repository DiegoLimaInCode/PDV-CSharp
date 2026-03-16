using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Produto : BaseEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
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
