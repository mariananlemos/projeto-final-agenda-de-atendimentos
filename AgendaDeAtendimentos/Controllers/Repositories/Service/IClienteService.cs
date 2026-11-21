using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.Services;

public enum ResultadoExclusao
{
    Excluido,
    NaoEncontrado,
    PossuiAgendamentos
}

public interface IClienteService
{
    Task<List<ClienteSaida>> ListarAsync(int profissionalId);
    Task<ClienteSaida?> BuscarPorIdAsync(int profissionalId, int id);
    Task<ClienteSaida> CadastrarAsync(int profissionalId, ClienteEntrada clienteEntrada);
    Task<ClienteSaida?> AtualizarAsync(int profissionalId, int id, ClienteEntrada clienteEntrada);
    Task<ResultadoExclusao> ExcluirAsync(int profissionalId, int id);
}
