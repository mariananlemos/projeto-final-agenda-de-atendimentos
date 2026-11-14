using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.ProfissionalService;

public interface IProfissionalService
{
    Task<ProfissionalCadastro?> BuscarProfissionalPorIdAsync(string login);
    Task<ProfissionalCadastro> CadastrarProfissionalAsync(ProfissionalCadastro profissionalCadastro);
}