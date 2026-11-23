using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Tests;

public class ClienteServiceTests : IDisposable
{
    private readonly BancoDeTeste _banco;
    private readonly ClienteService _service;
    private readonly int _profissionalA;
    private readonly int _profissionalB;

    public ClienteServiceTests()
    {
        _banco = new BancoDeTeste();
        _service = new ClienteService(new ClienteRepository(_banco.Context));
        _profissionalA = _banco.CriarProfissional("ana");
        _profissionalB = _banco.CriarProfissional("bia");
    }

    public void Dispose()
    {
        _banco.Dispose();
    }

    [Fact]
    public async Task Deve_Cadastrar_Cliente_E_Guardar_Telefone_Como_Informado()
    {
        var entrada = new ClienteEntrada { Nome = "Maria Silva", Telefone = "(11) 98888-7777" };

        var cliente = await _service.CadastrarAsync(_profissionalA, entrada);

        Assert.True(cliente.Id > 0);
        Assert.Equal("Maria Silva", cliente.Nome);
        Assert.Equal("(11) 98888-7777", cliente.Telefone);

        var buscado = await _service.BuscarPorIdAsync(_profissionalA, cliente.Id);
        Assert.NotNull(buscado);
        Assert.Equal("(11) 98888-7777", buscado!.Telefone);
    }

    [Fact]
    public async Task Deve_Editar_Cliente()
    {
        var cliente = await _service.CadastrarAsync(_profissionalA, new ClienteEntrada { Nome = "Maria", Telefone = "1111" });

        var editado = await _service.AtualizarAsync(_profissionalA, cliente.Id, new ClienteEntrada { Nome = "Maria Souza", Telefone = "2222" });

        Assert.NotNull(editado);
        Assert.Equal("Maria Souza", editado!.Nome);
        Assert.Equal("2222", editado.Telefone);
    }

    [Fact]
    public async Task Editar_Cliente_Inexistente_Retorna_Null()
    {
        var editado = await _service.AtualizarAsync(_profissionalA, 999, new ClienteEntrada { Nome = "Maria", Telefone = "1111" });

        Assert.Null(editado);
    }

    [Fact]
    public async Task Deve_Excluir_Cliente_Sem_Agendamentos()
    {
        var cliente = await _service.CadastrarAsync(_profissionalA, new ClienteEntrada { Nome = "Maria", Telefone = "1111" });

        var resultado = await _service.ExcluirAsync(_profissionalA, cliente.Id);

        Assert.Equal(ResultadoExclusao.Excluido, resultado);
        Assert.Null(await _service.BuscarPorIdAsync(_profissionalA, cliente.Id));
    }

    [Fact]
    public async Task Nao_Deve_Excluir_Cliente_Com_Agendamentos()
    {
        var cliente = await _service.CadastrarAsync(_profissionalA, new ClienteEntrada { Nome = "Maria", Telefone = "1111" });
        
        var servico = new Servico
        {
            ProfissionalId = _profissionalA,
            Nome = "Corte",
            DuracaoMinutos = 30,
            Preco = 50.00m
        };
        _banco.Context.Servicos.Add(servico);
        await _banco.Context.SaveChangesAsync();

        _banco.Context.Agendamentos.Add(new Agendamento
        {
            ProfissionalId = _profissionalA,
            ClienteId = cliente.Id,
            ServicoId = servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 9, 0, 0),
            DataHoraFim = new DateTime(2026, 11, 23, 10, 0, 0),
            PrecoCobrado = 50.00m,
            Status = StatusAgendamento.Agendado
        });
        await _banco.Context.SaveChangesAsync();

        var resultado = await _service.ExcluirAsync(_profissionalA, cliente.Id);

        Assert.Equal(ResultadoExclusao.PossuiAgendamentos, resultado);
        Assert.NotNull(await _service.BuscarPorIdAsync(_profissionalA, cliente.Id));
    }
    [Fact]
    public async Task Profissional_So_Lista_Os_Proprios_Clientes()
    {
        await _service.CadastrarAsync(_profissionalA, new ClienteEntrada { Nome = "Cliente da Ana", Telefone = "1111" });
        await _service.CadastrarAsync(_profissionalB, new ClienteEntrada { Nome = "Cliente da Bia", Telefone = "2222" });

        var clientesDaAna = await _service.ListarAsync(_profissionalA);

        Assert.Single(clientesDaAna);
        Assert.Equal("Cliente da Ana", clientesDaAna[0].Nome);
    }

    [Fact]
    public async Task Profissional_Nao_Acessa_Cliente_De_Outro_Profissional()
    {
        var clienteDaAna = await _service.CadastrarAsync(_profissionalA, new ClienteEntrada { Nome = "Cliente da Ana", Telefone = "1111" });

        // a Bia tenta buscar, editar e excluir o cliente da Ana
        var buscado = await _service.BuscarPorIdAsync(_profissionalB, clienteDaAna.Id);
        var editado = await _service.AtualizarAsync(_profissionalB, clienteDaAna.Id, new ClienteEntrada { Nome = "Hackeado", Telefone = "0000" });
        var excluido = await _service.ExcluirAsync(_profissionalB, clienteDaAna.Id);

        Assert.Null(buscado);
        Assert.Null(editado);
        Assert.Equal(ResultadoExclusao.NaoEncontrado, excluido);

        // o cliente da Ana continua igual
        var original = await _service.BuscarPorIdAsync(_profissionalA, clienteDaAna.Id);
        Assert.Equal("Cliente da Ana", original!.Nome);
    }
}
