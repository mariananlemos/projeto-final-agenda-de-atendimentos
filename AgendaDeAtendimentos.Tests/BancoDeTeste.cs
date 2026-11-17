using AgendaDeAtendimentos.Data;
using AgendaDeAtendimentos.ProfissionalModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AgendaDeAtendimentos.Tests;

// cria um banco SQLite em memória novo para cada teste
public class BancoDeTeste : IDisposable
{
    private readonly SqliteConnection _conexao;
    public AppDbContext Context {get; private set;}

    public BancoDeTeste()
    {
        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexao)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public int CriarProfissional(string login)
    {
        var profissional = new Profissional
        {
            Nome = "Profissional " + login,
            Login = login,
            SenhaHash = new byte[] { 1 },
            SenhaSalt = new byte[] { 1 }
        };
        Context.Profissionals.Add(profissional);
        Context.SaveChanges();
        return profissional.Id;
    }

    public void Dispose()
    {
        Context.Dispose();
        _conexao.Dispose();
    }
}
