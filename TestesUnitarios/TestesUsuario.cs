using Domain.Entities;
using Domain.Enuns;

namespace TestesUnitarios;

public class TestesUsuario
{
    [Fact]
    public void Deve_Criar_Usuario_Com_Sucesso()
    {
        string nome = "Gustavo";
        string email = "gustavo@gmail.com";
        string senha = "123456";
        DateOnly date = new DateOnly(2022, 1, 1);
        TipoAcesso tipoAcesso = TipoAcesso.Google;
    
        Usuario usuario = new Usuario(nome, email, senha, date, tipoAcesso);
    
        Assert.Equal(nome, usuario.Name);
        Assert.Equal(email, usuario.Email);
        Assert.Equal(senha, usuario.Senha);
        Assert.Equal(date, usuario.Nascimento);
        Assert.Equal(tipoAcesso, usuario.Acesso);
        Assert.Equal(Roles.User, usuario.Role);
        Assert.True(usuario.Status);
    }
    
    [Fact]
    public void Deve_Lancar_Excecao_Com_Mensagem_Correta_Quando_Nome_For_Vazio()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Usuario(
                "",
                "gustavo@gmail.com",
                "123456",
                new DateOnly(2022, 1, 1),
                TipoAcesso.Google
            )
        );

        Assert.Equal("Nome é obrigatório.", exception.Message);
    }
    
    [Fact]
    public void Deve_Lancar_Excecao_Com_Mensagem_Correta_Quando_Senha_For_Vazia()
    {
        var exception = Assert.Throws<BadRequestException>(() =>
            new Usuario(
                "Gustavo",
                "gustavo@gmail.com",
                "",
                new DateOnly(2022, 1, 1),
                TipoAcesso.Google
            )
        );

        Assert.Equal("Senha deve ter no mínimo 6 caracteres.", exception.Message);
    }
    
    [Fact]
    public void Deve_Lancar_Excecao_Com_Mensagem_Correta_Quando_Senha_For_Null()
    {
        var exception = Assert.Throws<BadRequestException>(() =>
            new Usuario(
                "Gustavo",
                "gustavo@gmail.com",
                null,
                new DateOnly(2022, 1, 1),
                TipoAcesso.Google
            )
        );

        Assert.Equal("Senha deve ter no mínimo 6 caracteres.", exception.Message);
    }
    
    [Fact]
    public void Deve_Lancar_Excecao_Com_Mensagem_Correta_Quando_Usuario_Ja_Estiver_Desativado()
    {
        Usuario usuario = new Usuario(
            "Gustavo",
            "gustavo@gmail.com",
            "123456",
            new DateOnly(2022, 1, 1),
            TipoAcesso.Google
        );
        
        usuario.Desativar();
            
        var exception = Assert.Throws<BadRequestException>(() =>
            usuario.Desativar()
        );

        Assert.Equal("Usuário já está desativado.", exception.Message);
    }

    [Fact]
    public void Deve_Lancar_Execao_Quando_Email_For_Invalido()
    {
        string nome = "Gustavo";
        string email = "email";
        string senha = "123456";
        DateOnly nascimento = new DateOnly(2022, 1, 1);
        TipoAcesso tipoAcesso = TipoAcesso.Google;
        
        var exception = Assert.Throws<BadRequestException>(() =>
            new Usuario(
                nome,
                email,
                senha,
                nascimento,
                tipoAcesso
            )
        );
        
        Assert.Equal("Email não está no padrão aceito.", exception.Message);
    }
    
    [Fact]
    public void Deve_Validar_Senha_Com_Sucesso()
    {
        string nome = "Gustavo";
        string email = "gustavo@gmail.com";
        string senha = "123456";
        DateOnly nascimento = new DateOnly(2022, 1, 1);
        TipoAcesso tipoAcesso = TipoAcesso.Google;
        string novaSenha = "123123";
        
        Usuario usuario = new Usuario(nome, email, senha, nascimento, tipoAcesso);
        
        usuario.AlterarSenha(novaSenha);
        
        Assert.Equal(novaSenha, usuario.Senha);
    }
}