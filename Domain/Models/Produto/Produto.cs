#nullable enable
using System.Collections.Generic;

namespace Domain.Models.Produto
{
    public class Produto
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal? Valor { get; private set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorParcelado { get; set; }
        public decimal Desconto { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int TipoProduto { get; set; }
        public bool NovoLancamento { get; set; }
        public bool? NovaGeracao { get; set; }
        public bool Disponivel { get; set; }
        public int MesesGarantia { get; set; }
        public int Quantidade { get; set; }
        public Informacoes? InformacoesAdicionais { get; set; }
        public int? InformacoesAdicionaisId { get; set; }

        public string Color { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public ICollection<Imagem.Imagem>? Imagens { get; set; } = new List<Imagem.Imagem>();
        public void CalcularValores()
        {
            Valor = ValorOriginal - Desconto;
        }
    }



    public class Informacoes
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
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

    public class Produtos
    {
        public List<Produto> ProdutosList { get; set; } = new List<Produto>();
    }
}
