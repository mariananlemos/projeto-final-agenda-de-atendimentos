using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public class ServicoRepository : IServicoRepository
{
    private readonly AppDbContext _context;
    public ServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Servico>> ListarPorProfissionalAsync(int profissionalId)
    {
        return await _context.Servicos
            .Where(s => s.ProfissionalId == profissionalId)
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<Servico?> BuscarPorIdAsync(int profissionalId, int id)
    {
        return await _context.Servicos
            .FirstOrDefaultAsync(s => s.ProfissionalId == profissionalId && s.Id == id);
    }

    public async Task CadastrarAsync(Servico servico)
    {
        _context.Servicos.Add(servico);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Servico servico)
    {
        _context.Servicos.Update(servico);
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Servico servico)
    {
        _context.Servicos.Remove(servico);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> PossuiAgendamentosAsync(int servicoId)
    {
        return await _context.Agendamentos.AnyAsync(a => a.ServicoId == servicoId);
    }
}