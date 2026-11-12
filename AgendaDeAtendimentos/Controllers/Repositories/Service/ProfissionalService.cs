using AgendaDeAtendimentos.ProfissionalController;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.ProfissionalRepository;
using Microsoft.AspNetCore.Identity.Data;

namespace AgendaDeAtendimentos.ProfissionalService;

public class ProfissionalService : IProfissionalService
{
    private readonly IProfissionalRepository _repository;
    public ProfissionalService(IProfissionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<Profissional> BuscarProfissionalPorIdAsync(string login)
    {
        return await _repository.BuscarProfissionalPorIdAsync(login);
    }
    public async Task<Profissional> CadastrarProfissionalAsync(Profissional profissional)
    {
        return await _repository.CadastrarProfissionalAsync(profissional);
    }
}
