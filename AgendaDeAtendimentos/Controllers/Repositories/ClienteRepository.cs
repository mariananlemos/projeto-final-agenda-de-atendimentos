using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;
    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    // todas as buscas filtram pelo profissional logado,
    // assim um profissional nunca enxerga cliente de outro
    public async Task<List<Cliente>> ListarPorProfissionalAsync(int profissionalId)
    {
        return await _context.Clientes
            .Where(c => c.ProfissionalId == profissionalId)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente?> BuscarPorIdAsync(int profissionalId, int id)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id && c.ProfissionalId == profissionalId);
    }

    public async Task<Cliente> CadastrarAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> PossuiAgendamentosAsync(int clienteId)
    {
        return await _context.Agendamentos.AnyAsync(a => a.ClienteId == clienteId);
    }
}
