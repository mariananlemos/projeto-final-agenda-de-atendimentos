using AgendaDeAtendimentos.ProfissionalController;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.ProfissionalRepository;
using AgendaDeAtendimentos.DTO;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Cryptography;
using System.Text;
using SQLitePCL;

namespace AgendaDeAtendimentos.ProfissionalService;

public class ProfissionalService : IProfissionalService
{
    private readonly IProfissionalRepository _repository;
    public ProfissionalService(IProfissionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfissionalCadastro?> BuscarProfissionalPorIdAsync(string login)
    {
        var profissional = await _repository.BuscarProfissionalPorIdAsync(login);
        if(profissional == null)
        {
            return null;
        }

        return new ProfissionalCadastro
        {
            Id = profissional.Id,
            Nome = profissional.Nome,
            Login = profissional.Login
        };
    }
    public async Task<ProfissionalCadastro> CadastrarProfissionalAsync(ProfissionalCadastro profissionalCadastro)
    {
        using var hmac = new HMACSHA512();
        byte[] senhaHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(profissionalCadastro.Senha));
        byte[] senhaSalt = hmac.Key;

        var profissional = new Profissional
        {
            Nome = profissionalCadastro.Nome,
            Login = profissionalCadastro.Login,
            SenhaHash = senhaHash,
            SenhaSalt = senhaSalt
        };

        var profissionalCadastrado = await _repository.CadastrarProfissionalAsync(profissional);
        return new ProfissionalCadastro
        {
            Id = profissionalCadastrado.Id,
            Nome = profissionalCadastrado.Nome,
            Login = profissionalCadastrado.Login
        };
    }
}
