using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Exceptions;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Tests;

public class AgendamentoServiceTests : IDisposable
{
    private const int Segunda = 1; // 23/11/2026 é segunda-feira

    private readonly BancoDeTeste _banco;
    private readonly AgendamentoService _service;
    private readonly ExpedienteService _expedienteService;
    private readonly Cliente _cliente;
    private readonly Servico _servico;
    private readonly int _profissionalA;

    public AgendamentoServiceTests()
    {
        _banco = new BancoDeTeste();
        _profissionalA = _banco.CriarProfissional("ana");

        var agendamentoRepo = new AgendamentoRepository(_banco.Context);
        var clienteRepo = new ClienteRepository(_banco.Context);
        var servicoRepo = new ServicoRepository(_banco.Context);
        var expedienteRepo = new ExpedienteRepository(_banco.Context);

        _expedienteService = new ExpedienteService(expedienteRepo);
        _service = new AgendamentoService(agendamentoRepo, clienteRepo, servicoRepo, _expedienteService);

        _cliente = new Cliente { ProfissionalId = _profissionalA, Nome = "Carla Dias", Telefone = "11988887777" };
        _banco.Context.Clientes.Add(_cliente);

        _servico = new Servico { ProfissionalId = _profissionalA, Nome = "Corte", DuracaoMinutos = 30, Preco = 50.00m };
        _banco.Context.Servicos.Add(_servico);

        _banco.Context.SaveChanges();
    }

    public void Dispose()
    {
        _banco.Dispose();
    }

    [Fact]
    public async Task Deve_Criar_Agendamento_Calculando_Fim_E_Congelando_Preco()
    {
        await _expedienteService.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "18:00" });

        var inicio = new DateTime(2026, 11, 23, 10, 0, 0);
        var resultado = await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = inicio
        });

        Assert.True(resultado.Id > 0);
        Assert.Equal(inicio, resultado.DataHoraInicio);
        Assert.Equal(new DateTime(2026, 11, 23, 10, 30, 0), resultado.DataHoraFim);
        Assert.Equal(50.00m, resultado.PrecoCobrado);
        Assert.Equal("Agendado", resultado.Status);

        // Alterar o preço do serviço depois não altera o PrecoCobrado do agendamento já salvo
        _servico.Preco = 90.00m;
        await _banco.Context.SaveChangesAsync();

        var salvoNoBanco = await _banco.Context.Agendamentos.FindAsync(resultado.Id);
        Assert.Equal(50.00m, salvoNoBanco!.PrecoCobrado);
    }

    [Fact]
    public async Task Deve_Bloqueiar_Agendamento_Fora_Do_Expediente()
    {
        await _expedienteService.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "12:00" });

        // Começa às 11:45 e termina às 12:15 (ultrapassa o expediente)
        var erro = await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.CriarAsync(_profissionalA, new AgendamentoEntrada
            {
                ClienteId = _cliente.Id,
                ServicoId = _servico.Id,
                DataHoraInicio = new DateTime(2026, 11, 23, 11, 45, 0)
            }));

        Assert.Equal("dataHoraInicio", erro.Campo);
    }

    [Fact]
    public async Task Deve_Bloquear_Conflito_De_Horario_Sobreposto()
    {
        await _expedienteService.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "18:00" });

        // Primeiro agendamento: 10:00 às 10:30
        await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0)
        });

        // Segundo agendamento: 10:15 às 10:45 (conflita!)
        var erro = await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.CriarAsync(_profissionalA, new AgendamentoEntrada
            {
                ClienteId = _cliente.Id,
                ServicoId = _servico.Id,
                DataHoraInicio = new DateTime(2026, 11, 23, 10, 15, 0)
            }));

        Assert.Equal("dataHoraInicio", erro.Campo);
    }

    [Fact]
    public async Task Deve_Permitir_Agendamento_Logo_Apos_Termino_Do_Anterior_Ou_Quando_Cancelado()
    {
        await _expedienteService.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "18:00" });

        // Agendamento 1: 10:00 às 10:30
        var primeiro = await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0)
        });

        // Agendamento 2: 10:30 às 11:00 (colado no término — deve permitir)
        var segundo = await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 30, 0)
        });
        Assert.True(segundo.Id > 0);

        // Cancela o primeiro e tenta agendar novamente às 10:00 (deve permitir)
        var entityPrimeiro = await _banco.Context.Agendamentos.FindAsync(primeiro.Id);
        entityPrimeiro!.Status = StatusAgendamento.Cancelado;
        await _banco.Context.SaveChangesAsync();

        var terceiro = await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0)
        });
        Assert.True(terceiro.Id > 0);
    }
    [Fact]
    public async Task Deve_Bloquear_Dia_Sem_Expediente_E_Manter_Horario_Ocupado_Quando_Status_Faltou()
    {
        await _expedienteService.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "18:00" });

        // 24/11/2026 é terça-feira (sem expediente)
        await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.CriarAsync(_profissionalA, new AgendamentoEntrada
            {
                ClienteId = _cliente.Id,
                ServicoId = _servico.Id,
                DataHoraInicio = new DateTime(2026, 11, 24, 10, 0, 0)
            }));

        // Cria agendamento na segunda às 10:00 e altera status para Faltou
        var criado = await _service.CriarAsync(_profissionalA, new AgendamentoEntrada
        {
            ClienteId = _cliente.Id,
            ServicoId = _servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0)
        });

        var entity = await _banco.Context.Agendamentos.FindAsync(criado.Id);
        entity!.Status = StatusAgendamento.Faltou;
        await _banco.Context.SaveChangesAsync();

        // Tentar agendar às 10:00 com o anterior como Faltou deve continuar bloqueado
        await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.CriarAsync(_profissionalA, new AgendamentoEntrada
            {
                ClienteId = _cliente.Id,
                ServicoId = _servico.Id,
                DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0)
            }));
    }
}