using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.Models;

// Versão simples do agendamento, só com o necessário para a regra
// "não excluir cliente com agendamentos". Vai ser completada na parte de agendamentos.
public class Agendamento
{
    public int Id {get; set;}
    [Required]
    public int ProfissionalId {get; set;}
    [Required]
    public int ClienteId {get; set;}
    public Cliente? Cliente {get; set;}
    public int ServicoId {get; set;}
    public Servico? Servico {get; set;}
    [Required]
    public DateTime DataHoraInicio {get; set;}
    [Required]
    public DateTime DataHoraFim {get; set;}
    [Required]
    public decimal PrecoCobrado {get; set;}
    [Required]
    public StatusAgendamento Status {get; set;} = StatusAgendamento.Agendado;
}
