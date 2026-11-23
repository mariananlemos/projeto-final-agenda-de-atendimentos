using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Tests;

public class ServicoServiceTests : IDisposable
{
    private readonly BancoDeTeste _banco;
    private readonly ServicoService _service;
    private readonly int _profissionalA;
    private readonly int _profissionalB;

    public ServicoServiceTests()
    {
        _banco = new BancoDeTeste();
        _service = new ServicoService(new ServicoRepository(_banco.Context));
        _profissionalA = _banco.CriarProfissional("ana");
        _profissionalB = _banco.CriarProfissional("bia");
    }

    public void Dispose()
    {
        _banco.Dispose();
    }

    [Fact]
    public async Task Deve_Cadastrar_Servico_Com_Sucesso()
    {
        var entrada = new ServicoEntrada
        {
            Nome = "Corte Feminino",
            DuracaoMinutos = 45,
            Preco = 80.00m
        };

        var servico = await _service.CadastrarAsync(_profissionalA, entrada);

        Assert.True(servico.Id > 0);
        Assert.Equal("Corte Feminino", servico.Nome);
        Assert.Equal(45, servico.DuracaoMinutos);
        Assert.Equal(80.00m, servico.Preco);
    }

    [Fact]
    public async Task Deve_Editar_Servico()
    {
        var servico = await _service.CadastrarAsync(_profissionalA, new ServicoEntrada
        {
            Nome = "Manicure",
            DuracaoMinutos = 30,
            Preco = 35.00m
        });

        var editado = await _service.AtualizarAsync(_profissionalA, servico.Id, new ServicoEntrada
        {
            Nome = "Manicure e Pedicure",
            DuracaoMinutos = 60,
            Preco = 65.00m
        });

        Assert.NotNull(editado);
        Assert.Equal("Manicure e Pedicure", editado!.Nome);
        Assert.Equal(60, editado.DuracaoMinutos);
        Assert.Equal(65.00m, editado.Preco);
    }

    [Fact]
    public async Task Deve_Excluir_Servico_Sem_Agendamentos()
    {
        var servico = await _service.CadastrarAsync(_profissionalA, new ServicoEntrada
        {
            Nome = "Escova",
            DuracaoMinutos = 30,
            Preco = 40.00m
        });

        var resultado = await _service.ExcluirAsync(_profissionalA, servico.Id);

        Assert.Equal(ResultadoExclusao.Excluido, resultado);
        Assert.Null(await _service.BuscarPorIdAsync(_profissionalA, servico.Id));
    }

    [Fact]
    public async Task Nao_Deve_Excluir_Servico_Com_Agendamentos()
    {
        var cliente = new Cliente { ProfissionalId = _profissionalA, Nome = "Maria", Telefone = "11999999999" };
        _banco.Context.Clientes.Add(cliente);
        await _banco.Context.SaveChangesAsync();

        var servico = await _service.CadastrarAsync(_profissionalA, new ServicoEntrada
        {
            Nome = "Hidratação",
            DuracaoMinutos = 40,
            Preco = 70.00m
        });

        _banco.Context.Agendamentos.Add(new Agendamento
        {
            ProfissionalId = _profissionalA,
            ClienteId = cliente.Id,
            ServicoId = servico.Id,
            DataHoraInicio = new DateTime(2026, 11, 23, 10, 0, 0),
            DataHoraFim = new DateTime(2026, 11, 23, 10, 40, 0),
            PrecoCobrado = 70.00m,
            Status = StatusAgendamento.Agendado
        });
        await _banco.Context.SaveChangesAsync();

        var resultado = await _service.ExcluirAsync(_profissionalA, servico.Id);

        Assert.Equal(ResultadoExclusao.PossuiAgendamentos, resultado);
        Assert.NotNull(await _service.BuscarPorIdAsync(_profissionalA, servico.Id));
    }

    [Fact]
    public async Task Profissional_Nao_Acessa_Nem_Exclui_Servico_De_Outro()
    {
        var servicoDaAna = await _service.CadastrarAsync(_profissionalA, new ServicoEntrada
        {
            Nome = "Design de Sobrancelha",
            DuracaoMinutos = 30,
            Preco = 45.00m
        });

        var buscado = await _service.BuscarPorIdAsync(_profissionalB, servicoDaAna.Id);
        var excluido = await _service.ExcluirAsync(_profissionalB, servicoDaAna.Id);

        Assert.Null(buscado);
        Assert.Equal(ResultadoExclusao.NaoEncontrado, excluido);
    }
    [Theory]
    [InlineData("A", 30, 50)]       // Nome com menos de 2 caracteres
    [InlineData("Corte", 12, 50)]   // Duração não múltipla de 5
    [InlineData("Corte", 30, -10)]  // Preço negativo
    public async Task Deve_Validar_Nome_Duracao_E_Preco_Invalidos(string nome, int duracao, decimal preco)
    {
        var entrada = new ServicoEntrada { Nome = nome, DuracaoMinutos = duracao, Preco = preco };
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(entrada);
        var resultados = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        var dtoValido = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(entrada, context, resultados, true);

        if (dtoValido)
        {
            await Assert.ThrowsAsync<AgendaDeAtendimentos.Exceptions.ValidacaoException>(() =>
                _service.CadastrarAsync(_profissionalA, entrada));
        }
        else
        {
            Assert.NotEmpty(resultados);
        }
    }
}