namespace Domain.Models.Produto;

public class InformacoesProduto
{
    public Guid Id { get; set; }
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