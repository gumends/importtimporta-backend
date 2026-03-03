namespace Domain.Models.carrinho;

public class Carrinho
{
    public Guid Id { get; set; }
    public Guid IdProduto{ get; set; }
    public int Quantidade { get; set; }
    public Guid IdUsuario { get; set; }
    public DateTime CriadoEm { get; set; }
}