namespace AgendaDeAtendimentos.Models;

public class Expediente
{
    public int Id {get; set;}
    public int ProfissionalId {get; set;}

    // 0 = domingo, 1 = segunda ... 6 = sábado (igual ao DayOfWeek do C#)
    public int DiaSemana {get; set;}

    public TimeOnly HoraInicio {get; set;}
    public TimeOnly HoraFim {get; set;}
}
