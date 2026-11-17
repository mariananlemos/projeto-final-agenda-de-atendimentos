using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public interface IExpedienteRepository
{
    Task<List<Expediente>> ListarPorProfissionalAsync(int profissionalId);
    Task<Expediente?> BuscarPorDiaAsync(int profissionalId, int diaSemana);
    Task<Expediente> CadastrarAsync(Expediente expediente);
    Task AtualizarAsync(Expediente expediente);
    Task ExcluirAsync(Expediente expediente);
}
