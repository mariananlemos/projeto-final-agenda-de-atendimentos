using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.Models;

namespace AgendaDeAtendimentos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Profissional> Profissionals {get; set;}
    public DbSet<Cliente> Clientes {get; set;}
    public DbSet<Expediente> Expedientes {get; set;}
    public DbSet<Agendamento> Agendamentos {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(cliente =>
        {
            cliente.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            cliente.Property(c => c.Telefone).HasMaxLength(20).IsRequired();
            cliente.HasOne<Profissional>()
                .WithMany()
                .HasForeignKey(c => c.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Expediente>(expediente =>
        {
            expediente.HasOne<Profissional>()
                .WithMany()
                .HasForeignKey(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);

            // um profissional só pode ter um expediente por dia da semana
            expediente.HasIndex(e => new { e.ProfissionalId, e.DiaSemana }).IsUnique();
        });

        modelBuilder.Entity<Agendamento>(agendamento =>
        {
            // Restrict: o banco também não deixa apagar cliente que tem agendamento
            agendamento.HasOne(a => a.Cliente)
                .WithMany()
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
