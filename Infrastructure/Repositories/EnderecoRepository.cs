using Application.Interfaces.Repositories;
using Domain.Models.Endereco;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnderecoRepository : IEnderecoRepository
{
    private readonly AppDbContext _db;

    public EnderecoRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<Endereco> CadastrarEndereco(Endereco endereco)
    {
        await _db.Enderecos.AddAsync(endereco);
        _db.SaveChanges();
        return endereco;
    }

    public async Task<Endereco> BuscaEndereco(Guid id)
    {
        var endereco = await _db.Enderecos.FindAsync(id);
        if (endereco == null)
            return null;
        return endereco;
    }

    public async Task<List<Endereco>> BuscaTodosEnderecos(Guid usuarioId)
    {
        var enderecos = await _db.Enderecos.Where(e => e.IdUsuario == usuarioId).ToListAsync();
        return enderecos ;
    }

    public async Task<Endereco> AtualizarEndereco(EnderecoRequest newEndereco, Guid id)
    {
        var endereco = await _db.Enderecos.FindAsync(id);
            
        if (endereco == null)
            return null;
            
        endereco.Cep = newEndereco.Cep;
        endereco.Complemento = newEndereco.Complemento;
        endereco.Numero = newEndereco.Numero;
        endereco.Logradouro = newEndereco.Logradouro;
            
        _db.Enderecos.Update(endereco);
        await _db.SaveChangesAsync();
            
        return endereco;
    }

    public async Task<Endereco> ExcluirEndereco(Guid id)
    {
        var endereco = await _db.Enderecos.FindAsync(id);
        if (endereco == null)
            return null;

        _db.Enderecos.Remove(endereco);
        await _db.SaveChangesAsync();
        return endereco;
    }
}