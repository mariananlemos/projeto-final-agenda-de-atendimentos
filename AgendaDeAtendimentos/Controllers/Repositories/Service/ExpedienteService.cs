using System.Globalization;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Exceptions;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;

namespace AgendaDeAtendimentos.Services;

public class ExpedienteService : IExpedienteService
{
    private static readonly string[] NomesDias =
    {
        "Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado"
    };

    private readonly IExpedienteRepository _repository;
    public ExpedienteService(IExpedienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ExpedienteSaida>> ListarAsync(int profissionalId)
    {
        var expedientes = await _repository.ListarPorProfissionalAsync(profissionalId);
        return expedientes.Select(e => ParaSaida(e)).ToList();
    }

    // o PUT funciona como "cadastrar ou alterar": se o dia já existe, atualiza
    public async Task<ExpedienteSaida> SalvarAsync(int profissionalId, int dia, ExpedienteEntrada expedienteEntrada)
    {
        ValidarDia(dia);

        var horaInicio = ConverterHora(expedienteEntrada.HoraInicio, "horaInicio");
        var horaFim = ConverterHora(expedienteEntrada.HoraFim, "horaFim");

        if(horaFim <= horaInicio)
        {
            throw new ValidacaoException("horaFim", "A hora de fim precisa ser depois da hora de início.");
        }

        var expediente = await _repository.BuscarPorDiaAsync(profissionalId, dia);
        if(expediente == null)
        {
            expediente = new Expediente
            {
                ProfissionalId = profissionalId,
                DiaSemana = dia,
                HoraInicio = horaInicio,
                HoraFim = horaFim
            };
            await _repository.CadastrarAsync(expediente);
        }
        else
        {
            expediente.HoraInicio = horaInicio;
            expediente.HoraFim = horaFim;
            await _repository.AtualizarAsync(expediente);
        }

        return ParaSaida(expediente);
    }

    public async Task<bool> ExcluirAsync(int profissionalId, int dia)
    {
        ValidarDia(dia);

        var expediente = await _repository.BuscarPorDiaAsync(profissionalId, dia);
        if(expediente == null)
        {
            return false;
        }

        await _repository.ExcluirAsync(expediente);
        return true;
    }

    public async Task<bool> PodeAtenderAsync(int profissionalId, DateTime inicio, DateTime fim)
    {
        // o atendimento precisa começar e terminar no mesmo dia
        if(inicio.Date != fim.Date || fim <= inicio)
        {
            return false;
        }

        var expediente = await _repository.BuscarPorDiaAsync(profissionalId, (int)inicio.DayOfWeek);
        if(expediente == null)
        {
            return false; // dia sem expediente
        }

        var horaInicio = TimeOnly.FromDateTime(inicio);
        var horaFim = TimeOnly.FromDateTime(fim);
        return horaInicio >= expediente.HoraInicio && horaFim <= expediente.HoraFim;
    }

    private static void ValidarDia(int dia)
    {
        if(dia < 0 || dia > 6)
        {
            throw new ValidacaoException("dia", "O dia deve ser um número de 0 (domingo) a 6 (sábado).");
        }
    }

    private static TimeOnly ConverterHora(string hora, string campo)
    {
        if(!TimeOnly.TryParseExact(hora, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultado))
        {
            throw new ValidacaoException(campo, "A hora deve estar no formato HH:mm.");
        }
        return resultado;
    }

    private static ExpedienteSaida ParaSaida(Expediente expediente)
    {
        return new ExpedienteSaida
        {
            DiaSemana = expediente.DiaSemana,
            NomeDia = NomesDias[expediente.DiaSemana],
            HoraInicio = expediente.HoraInicio.ToString("HH:mm"),
            HoraFim = expediente.HoraFim.ToString("HH:mm")
        };
    }
}
