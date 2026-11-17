using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Exceptions;
using AgendaDeAtendimentos.Repositories;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Tests;

public class ExpedienteServiceTests : IDisposable
{
    private const int Segunda = 1;
    private const int Terca = 2;

    private readonly BancoDeTeste _banco;
    private readonly ExpedienteService _service;
    private readonly int _profissionalA;
    private readonly int _profissionalB;

    public ExpedienteServiceTests()
    {
        _banco = new BancoDeTeste();
        _service = new ExpedienteService(new ExpedienteRepository(_banco.Context));
        _profissionalA = _banco.CriarProfissional("ana");
        _profissionalB = _banco.CriarProfissional("bia");
    }

    public void Dispose()
    {
        _banco.Dispose();
    }

    [Fact]
    public async Task Deve_Cadastrar_Expediente()
    {
        var expediente = await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        Assert.Equal(Segunda, expediente.DiaSemana);
        Assert.Equal("Segunda-feira", expediente.NomeDia);
        Assert.Equal("08:00", expediente.HoraInicio);
        Assert.Equal("17:00", expediente.HoraFim);
        Assert.Single(await _service.ListarAsync(_profissionalA));
    }

    [Fact]
    public async Task Deve_Alterar_Expediente_Do_Mesmo_Dia()
    {
        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "10:00", HoraFim = "19:00" });

        var lista = await _service.ListarAsync(_profissionalA);
        Assert.Single(lista); // não duplicou
        Assert.Equal("10:00", lista[0].HoraInicio);
        Assert.Equal("19:00", lista[0].HoraFim);
    }

    [Fact]
    public async Task Deve_Excluir_Expediente()
    {
        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        var excluido = await _service.ExcluirAsync(_profissionalA, Segunda);

        Assert.True(excluido);
        Assert.Empty(await _service.ListarAsync(_profissionalA));
    }

    [Fact]
    public async Task Excluir_Dia_Sem_Expediente_Retorna_False()
    {
        var excluido = await _service.ExcluirAsync(_profissionalA, Terca);

        Assert.False(excluido);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    public async Task Dia_Invalido_Deve_Dar_Erro_No_Campo_Dia(int dia)
    {
        var erro = await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.SalvarAsync(_profissionalA, dia, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" }));

        Assert.Equal("dia", erro.Campo);
    }

    [Fact]
    public async Task Hora_Fim_Antes_Do_Inicio_Deve_Dar_Erro_No_Campo_HoraFim()
    {
        var erro = await Assert.ThrowsAsync<ValidacaoException>(() =>
            _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "17:00", HoraFim = "08:00" }));

        Assert.Equal("horaFim", erro.Campo);
    }

    [Fact]
    public async Task Profissional_Nao_Ve_Nem_Exclui_Expediente_De_Outro()
    {
        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        var listaDaBia = await _service.ListarAsync(_profissionalB);
        var excluidoPelaBia = await _service.ExcluirAsync(_profissionalB, Segunda);

        Assert.Empty(listaDaBia);
        Assert.False(excluidoPelaBia);
        Assert.Single(await _service.ListarAsync(_profissionalA));
    }

    [Fact]
    public async Task Dia_Sem_Expediente_Nao_Permite_Atendimento()
    {
        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        // 24/11/2026 é uma terça-feira, e a Ana só atende na segunda
        var pode = await _service.PodeAtenderAsync(_profissionalA, new DateTime(2026, 11, 24, 9, 0, 0), new DateTime(2026, 11, 24, 10, 0, 0));

        Assert.False(pode);
    }

    [Fact]
    public async Task Dia_Com_Expediente_Permite_Atendimento_Dentro_Do_Horario()
    {
        await _service.SalvarAsync(_profissionalA, Segunda, new ExpedienteEntrada { HoraInicio = "08:00", HoraFim = "17:00" });

        // 23/11/2026 é uma segunda-feira
        var dentro = await _service.PodeAtenderAsync(_profissionalA, new DateTime(2026, 11, 23, 9, 0, 0), new DateTime(2026, 11, 23, 10, 0, 0));
        var fora = await _service.PodeAtenderAsync(_profissionalA, new DateTime(2026, 11, 23, 16, 30, 0), new DateTime(2026, 11, 23, 17, 30, 0));

        Assert.True(dentro);
        Assert.False(fora);
    }
}
