using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.Services;

public interface IServicoService
{
    Task<List<ServicoSaida>> ListarAsync(int profissionalId);
    Task<ServicoSaida?> BuscarPorIdAsync(int profissionalId, int id);
    Task<ServicoSaida> CadastrarAsync(int profissionalId, ServicoEntrada entrada);
    Task<ServicoSaida?> AtualizarAsync(int profissionalId, int id, ServicoEntrada entrada);
    Task<ResultadoExclusao> ExcluirAsync(int profissionalId, int id);
}