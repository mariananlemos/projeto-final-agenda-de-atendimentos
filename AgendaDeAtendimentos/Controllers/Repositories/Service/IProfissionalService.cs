using AgendaDeAtendimentos.ProfissionalModels;

namespace AgendaDeAtendimentos.ProfissionalService;

public interface IProfissionalService
{
    Task<Profissional> BuscarProfissionalPorIdAsync(string login);
    Task<Profissional> CadastrarProfissionalAsync(Profissional profissional);
}