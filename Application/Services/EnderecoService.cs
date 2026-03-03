using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Endereco;

namespace Application.Services;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;

    public EnderecoService(IEnderecoRepository enderecoRepository)
    {
        _enderecoRepository = enderecoRepository;
    }
    
    public async Task<Endereco> CadastrarEndereco(EnderecoRequest enderecoRequest, Guid usuarioId)
    {
        var endereco = new Endereco();
            
        endereco.Logradouro = enderecoRequest.Logradouro;
        endereco.Numero = enderecoRequest.Numero;
        endereco.Complemento = enderecoRequest.Complemento;
        endereco.Cep = enderecoRequest.Cep;
        endereco.IdUsuario = usuarioId;
            
        return await _enderecoRepository.CadastrarEndereco(endereco);
    }

    public async Task<List<Endereco>> BuscaEnderecos(Guid usuarioId)
    {
        return await _enderecoRepository.BuscaTodosEnderecos(usuarioId);
    }

    public async Task<Endereco> BuscaEndereco(Guid id)
    {
        Endereco endereco = await _enderecoRepository.BuscaEndereco(id);
        if (endereco == null)
        {
            throw new BadRequestException("Endereço não encontrado.");
        }
        return endereco;
    }

    public async Task<Endereco> AtualizarEndereco(EnderecoRequest enderecoRequest, Guid id)
    {
        return await _enderecoRepository.AtualizarEndereco(enderecoRequest, id);
    }

    public async Task<Endereco> ExcluirEndereco(Guid id)
    {
        var endereco = await _enderecoRepository.ExcluirEndereco(id);
        if (endereco == null)
        {
            throw new BadRequestException("Endereço não encontrado.");
        }
        return endereco;
    }
}