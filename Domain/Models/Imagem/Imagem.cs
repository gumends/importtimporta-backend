using System.Text.Json.Serialization;

namespace Domain.Models.Imagem
{
    public class Imagem
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Caminho { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public Guid ProdutoId { get; set; }
        [JsonIgnore]
        public Produto.Produto? Produto { get; set; }

        public Imagem(string caminho, string descricao, Guid produtoId)
        {
            Caminho = caminho;
            Descricao = descricao;
            ProdutoId = produtoId;
        }
    }
}
