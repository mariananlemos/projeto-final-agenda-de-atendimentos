using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Exceptions;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;

namespace AgendaDeAtendimentos.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IServicoRepository _servicoRepository;
    private readonly IExpedienteService _expedienteService;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository,
        IServicoRepository servicoRepository,
        IExpedienteService expedienteService)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
        _servicoRepository = servicoRepository;
        _expedienteService = expedienteService;
    }

    public async Task<AgendamentoSaida> CriarAsync(int profissionalId, AgendamentoEntrada entrada)
    {
        // 1. Verifica se o cliente existe e pertence à profissional logada
        var cliente = await _clienteRepository.BuscarPorIdAsync(profissionalId, entrada.ClienteId);
        if (cliente == null)
        {
            throw new ValidacaoException("clienteId", "Cliente não encontrado.");
        }

        // 2. Verifica se o serviço existe e pertence à profissional logada
        var servico = await _servicoRepository.BuscarPorIdAsync(profissionalId, entrada.ServicoId);
        if (servico == null)
        {
            throw new ValidacaoException("servicoId", "Serviço não encontrado.");
        }

        // 3. Regra 3 e 4: Calcula o horário de fim na criação e congela o preço atual do serviço
        var dataHoraInicio = entrada.DataHoraInicio;
        var dataHoraFim = dataHoraInicio.AddMinutes(servico.DuracaoMinutos);
        var precoCobrado = servico.Preco;

        // 4. Regra 6: Confere se o atendimento cabe no expediente da profissional
        var cabeNoExpediente = await _expedienteService.PodeAtenderAsync(profissionalId, dataHoraInicio, dataHoraFim);
        if (!cabeNoExpediente)
        {
            throw new ValidacaoException("dataHoraInicio", "O horário está fora do expediente ou em um dia sem atendimento.");
        }

        // 5. Regra 1 e 2: Verifica conflito com agendamentos ativos (ignora Cancelados; Faltou continua ocupando)
        var agendamentosDoDia = await _agendamentoRepository.ListarAtivosPorDataAsync(profissionalId, dataHoraInicio.Date);
        bool temConflito = agendamentosDoDia.Any(existente =>
            dataHoraInicio < existente.DataHoraFim && dataHoraFim > existente.DataHoraInicio);

        if (temConflito)
        {
            throw new ValidacaoException("dataHoraInicio", "Já existe um atendimento agendado neste intervalo.");
        }

        // 6. Cria e salva como Agendado
        var agendamento = new Agendamento
        {
            ProfissionalId = profissionalId,
            ClienteId = cliente.Id,
            ServicoId = servico.Id,
            DataHoraInicio = dataHoraInicio,
            DataHoraFim = dataHoraFim,
            PrecoCobrado = precoCobrado,
            Status = StatusAgendamento.Agendado
        };

        await _agendamentoRepository.CadastrarAsync(agendamento);

        return new AgendamentoSaida
        {
            Id = agendamento.Id,
            ClienteId = cliente.Id,
            NomeCliente = cliente.Nome,
            ServicoId = servico.Id,
            NomeServico = servico.Nome,
            DataHoraInicio = agendamento.DataHoraInicio,
            DataHoraFim = agendamento.DataHoraFim,
            PrecoCobrado = agendamento.PrecoCobrado,
            Status = agendamento.Status.ToString()
        };
    }
}