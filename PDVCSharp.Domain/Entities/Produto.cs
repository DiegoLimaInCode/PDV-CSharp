using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    public class Produto : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Quantity { get; set; }
        public string ImagePath { get; set; }

        public Produto()
        {
            Price = 0;
        }
    }
}
