namespace AgendaDeAtendimentos.Models;

// Versão simples do agendamento, só com o necessário para a regra
// "não excluir cliente com agendamentos". Vai ser completada na parte de agendamentos.
public class Agendamento
{
    public int Id {get; set;}
    public int ProfissionalId {get; set;}
    public int ClienteId {get; set;}
    public Cliente? Cliente {get; set;}
    public DateTime DataHoraInicio {get; set;}
    public DateTime DataHoraFim {get; set;}
}
