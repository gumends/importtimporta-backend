using Domain.Models.carrinho;
using Domain.Models.Produto;

namespace Domain.DTO;

public class CarrinhoDto
{
    public decimal Subtotal  { get; set; }
    public decimal Total { get; set; }
    public decimal TaxaEntrega { get; set; }
    public List<NewCarrinhoDto> Carrinhos { get; set; }
}

public class NewCarrinhoDto :  Carrinho
{
    public Produto Produto { get; set; }
}