using AgendaDeAtendimentos.ProfissionalModels;

namespace AgendaDeAtendimentos.ProfissionalRepository;

public interface IProfissionalRepository
{
    Task<Profissional?> BuscarProfissionalPorIdAsync(string login);
    Task<Profissional> CadastrarProfissionalAsync(Profissional profissional);
}