using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Domain.Enuns;

namespace Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Senha { get; private set; }
    public DateOnly Nascimento { get; private set; }
    public TipoAcesso Acesso { get; private set; }
    public Roles Role { get; private set; }
    public bool Status { get; private set; }

    protected Usuario() { }

    public Usuario(
        string name,
        string email,
        string senha,
        DateOnly nascimento,
        TipoAcesso acesso)
    {
        ValidarNome(name);
        ValidarEmail(email);
        ValidarSenha(senha);

        Name = name;
        Email = email;
        Senha = senha;
        Nascimento = nascimento;
        Acesso = acesso;
        Role = Roles.User;
        Status = true;
    }

    public void AtualizarDados(
        string name,
        string email,
        DateOnly nascimento
        )
    {
        ValidarNome(name);
        ValidarEmail(email);

        Name = name;
        Email = email;
        Nascimento = nascimento;
    }

    public void AlterarSenha(string novaSenha)
    {
        ValidarSenha(novaSenha);
        Senha = novaSenha;
    }

    public void AlterarRole(Roles role)
    {
        Role = role;
    }

    public void Desativar()
    {
        if (!Status)
            throw new BadRequestException("Usuário já está desativado.");

        Status = false;
    }

    public void Ativar()
    {
        if (Status)
            throw new BadRequestException("Usuário já está ativo.");

        Status = true;
    }

    private void ValidarNome(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Nome é obrigatório.");
    }

    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BadRequestException("Email é obrigatório.");

        var emailAttribute = new EmailAddressAttribute();

        if (!emailAttribute.IsValid(email))
            throw new BadRequestException("Email não está no padrão aceito.");
    }

    private void ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new BadRequestException("Senha deve ter no mínimo 6 caracteres.");
    }
}