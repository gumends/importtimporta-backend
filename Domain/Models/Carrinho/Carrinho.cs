namespace Domain.Models.carrinho;

public class Carrinho
{
    public int Id { get; set; }
    public int IdProduto{ get; set; }
    public int Quantidade { get; set; }
    public int IdUsuario { get; set; }
    public DateTime CriadoEm { get; set; }
}