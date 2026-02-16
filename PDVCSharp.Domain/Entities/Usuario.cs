using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{

    public class Usuario
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
        
        [JsonPropertyName("isDeleted")]
        public bool Deleted { get; set; }

    }

}
