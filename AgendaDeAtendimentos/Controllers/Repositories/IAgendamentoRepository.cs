using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public interface IAgendamentoRepository
{
    Task<List<Agendamento>> ListarAtivosPorDataAsync(int profissionalId, DateTime data);
    Task CadastrarAsync(Agendamento agendamento);
}