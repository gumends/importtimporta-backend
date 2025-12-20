namespace Domain.Models.Produto
{
    public class ProdutoResponseData
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal? Valor { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorParcelado { get; set; }
        public decimal Desconto { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int TipoProduto { get; set; }
        public bool NovoLancamento { get; set; }
        public bool? NovaGeracao { get; set; }
        public bool Disponivel { get; set; }
        public int MesesGarantia { get; set; }
        public InformacoesResponse? InformacoesAdicionais { get; set; }
        public int? InformacoesAdicionaisId { get; set; }
        public string Color { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public List<string> Imagens { get; set; }
        public void CalcularValores()
        {
            this.Valor = ValorOriginal - Desconto;
        }
    }

    public class InformacoesResponse
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string? ArmazenamentoInterno { get; set; }
        public string? TipoTela { get; set; }
        public string? TamanhoTela { get; set; }
        public string? ResolucaoTela { get; set; }
        public string? Tecnologia { get; set; }
        public string? Processador { get; set; }
        public string? SistemaOperacional { get; set; }
        public string? CameraTraseira { get; set; }
        public string? CameraFrontal { get; set; }
        public string? Bateria { get; set; }
        public string? QuantidadeChips { get; set; }
        public string? Material { get; set; }
    }

    public class ProdutosResponse
    {
        public List<Produto> ProdutosList { get; set; } = new List<Produto>();
    }
}
