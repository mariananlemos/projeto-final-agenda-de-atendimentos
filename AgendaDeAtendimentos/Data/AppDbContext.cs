using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Profissional> Profissionals {get; set;}
}