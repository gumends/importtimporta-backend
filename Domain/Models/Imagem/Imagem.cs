using System.Text.Json.Serialization;

namespace Domain.Models.Imagem
{
    public class Imagem
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Caminho { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        // FK obrigatória
        public int ProdutoId { get; set; }

        // Navegação, ignorar no JSON, mas tem que ser nullable
        [JsonIgnore]
        public Produto.Produto? Produto { get; set; }
    }
}
