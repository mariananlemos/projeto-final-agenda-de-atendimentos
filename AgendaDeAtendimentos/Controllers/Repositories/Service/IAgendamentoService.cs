using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.Services;

public interface IAgendamentoService
{
    Task<AgendamentoSaida> CriarAsync(int profissionalId, AgendamentoEntrada entrada);
}