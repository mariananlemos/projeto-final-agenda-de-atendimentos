using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.ProfissionalController;

namespace AgendaDeAtendimentos.ProfissionalRepository;

public class ProfissionalRepository : IProfissionalRepository
{
    private readonly AppDbContext _context;
    public ProfissionalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Profissional?> BuscarProfissionalPorIdAsync(string login)
    {
        return await _context.Profissionals.FirstOrDefaultAsync(p => p.Login.ToLower() == login.ToLower());
    }

    public async Task<Profissional> CadastrarProfissionalAsync(Profissional profissional)
    {
        _context.Profissionals.Add(profissional);
        await _context.SaveChangesAsync();
        return profissional;
    }
}