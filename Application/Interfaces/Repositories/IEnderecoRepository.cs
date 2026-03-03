using Domain.Models.Endereco;

namespace Application.Interfaces.Repositories;

public interface IEnderecoRepository
{
    Task<Endereco> CadastrarEndereco(Endereco endereco);

    Task<Endereco> BuscaEndereco(Guid id);

    Task<List<Endereco>> BuscaTodosEnderecos(Guid usuarioId);

    Task<Endereco> AtualizarEndereco(EnderecoRequest newEndereco, Guid id);

    Task<Endereco> ExcluirEndereco(Guid id);
}