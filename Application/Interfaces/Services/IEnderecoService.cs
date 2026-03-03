using Domain.Models.Endereco;

namespace Application.Interfaces.Services;

public interface IEnderecoService
{
    Task<Endereco> CadastrarEndereco(EnderecoRequest enderecoRequest, Guid usuarioId);

    Task<List<Endereco>> BuscaEnderecos(Guid usuarioId);

    Task<Endereco> BuscaEndereco(Guid id);

    Task<Endereco> AtualizarEndereco(EnderecoRequest enderecoRequest, Guid id);

    Task<Endereco> ExcluirEndereco(Guid id);
}