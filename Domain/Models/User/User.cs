using System.Text.Json.Serialization;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    [JsonIgnore]
    public string Senha { get; set; } = "";
    public DateOnly Nascimento { get; set; } = new DateOnly();
    public TipoAcesso Acesso { get; set; }
    public Roles Role { get; set; } = Roles.User;
    public bool Status { get; set; } = true;
}

public enum TipoAcesso
{
    Google = 1,
    Apple = 2,
    Padrao = 3
}

public enum Roles
{
    Admin = 1,
    User = 2
}
