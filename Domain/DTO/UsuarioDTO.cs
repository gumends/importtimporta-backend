using Domain.Enuns;

namespace Domain.DTO;

public class UsuarioDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public DateOnly Nascimento { get; set; }
    public TipoAcesso Acesso { get; set; }
    public Roles Role { get; set; }
    public bool Status { get; set; }
}