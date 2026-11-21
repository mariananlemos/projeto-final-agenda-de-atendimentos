using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.Services;

public interface IExpedienteService
{
    Task<List<ExpedienteSaida>> ListarAsync(int profissionalId);
    Task<ExpedienteSaida> SalvarAsync(int profissionalId, int dia, ExpedienteEntrada expedienteEntrada);
    Task<bool> ExcluirAsync(int profissionalId, int dia);

    // vai ser usado na parte de agendamentos: dia sem expediente não permite atendimento
    Task<bool> PodeAtenderAsync(int profissionalId, DateTime inicio, DateTime fim);
}
