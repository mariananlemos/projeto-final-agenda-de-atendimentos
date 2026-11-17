using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public class ExpedienteRepository : IExpedienteRepository
{
    private readonly AppDbContext _context;
    public ExpedienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expediente>> ListarPorProfissionalAsync(int profissionalId)
    {
        return await _context.Expedientes
            .Where(e => e.ProfissionalId == profissionalId)
            .OrderBy(e => e.DiaSemana)
            .ToListAsync();
    }

    public async Task<Expediente?> BuscarPorDiaAsync(int profissionalId, int diaSemana)
    {
        return await _context.Expedientes
            .FirstOrDefaultAsync(e => e.ProfissionalId == profissionalId && e.DiaSemana == diaSemana);
    }

    public async Task<Expediente> CadastrarAsync(Expediente expediente)
    {
        _context.Expedientes.Add(expediente);
        await _context.SaveChangesAsync();
        return expediente;
    }

    public async Task AtualizarAsync(Expediente expediente)
    {
        _context.Expedientes.Update(expediente);
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Expediente expediente)
    {
        _context.Expedientes.Remove(expediente);
        await _context.SaveChangesAsync();
    }
}
