using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public interface IServicoRepository
{
    Task<List<Servico>> ListarPorProfissionalAsync(int profissionalId);
    Task<Servico?> BuscarPorIdAsync(int profissionalId, int id);
    Task CadastrarAsync(Servico servico);
    Task AtualizarAsync(Servico servico);
    Task ExcluirAsync(Servico servico);
    Task<bool> PossuiAgendamentosAsync(int servicoId);
}