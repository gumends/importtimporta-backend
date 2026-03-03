using Domain.Models.Produto;

namespace Domain.DTO;

public class ProdutoDto
{
        public string NomeProduto { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorParcelado { get; set; }
        public string Descricao { get; set; }
        public int TipoProduto { get; set; }
        public bool Disponivel { get; set; }
        public int MesesGarantia { get; set; }
        public int Quantidade { get; set; }
        public string Color { get; set; }
        public string ColorName { get; set; }
        public InformacoesProduto InformacoesProduto { get; set; }
}
