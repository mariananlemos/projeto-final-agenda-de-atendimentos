using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public interface IClienteRepository
{
    Task<List<Cliente>> ListarPorProfissionalAsync(int profissionalId);
    Task<Cliente?> BuscarPorIdAsync(int profissionalId, int id);
    Task<Cliente> CadastrarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task ExcluirAsync(Cliente cliente);
    Task<bool> PossuiAgendamentosAsync(int clienteId);
}
