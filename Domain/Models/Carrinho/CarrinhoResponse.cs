using Domain.Models.carrinho;
using Domain.Models.Produto;

namespace Domain.DTO;

public class CarrinhoResponse
{
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public decimal TaxaEntrega { get; set; }
    public List<CarrinhoItemResponse> Itens { get; set; } = new();
}

public class CarrinhoItemResponse
{
    public Guid IdProduto { get; set; }
    public string NomeProduto { get; set; }
    public decimal ValorUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal TotalItem { get; set; }
    public string ImagemUrl { get; set; }
    public string Descricao { get; set; }
}