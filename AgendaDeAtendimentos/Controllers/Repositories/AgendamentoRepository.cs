using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AppDbContext _context;
    public AgendamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    // Busca todos os agendamentos do dia que NÃO foram cancelados (Agendado, Concluído e Faltou ocupam horário)
    public async Task<List<Agendamento>> ListarAtivosPorDataAsync(int profissionalId, DateTime data)
    {
        var inicioDia = data.Date;
        var fimDia = inicioDia.AddDays(1);

        return await _context.Agendamentos
            .Where(a => a.ProfissionalId == profissionalId
                     && a.Status != StatusAgendamento.Cancelado
                     && a.DataHoraInicio >= inicioDia
                     && a.DataHoraInicio < fimDia)
            .ToListAsync();
    }

    public async Task CadastrarAsync(Agendamento agendamento)
    {
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
    }
}