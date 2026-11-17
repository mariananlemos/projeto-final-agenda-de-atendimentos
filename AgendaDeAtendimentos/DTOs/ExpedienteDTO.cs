using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.DTO;

public class ExpedienteEntrada
{
    [Required(ErrorMessage = "A hora de início é um campo obrigatório.")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "A hora de início deve estar no formato HH:mm.")]
    public string HoraInicio {get; set;} = string.Empty;

    [Required(ErrorMessage = "A hora de fim é um campo obrigatório.")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "A hora de fim deve estar no formato HH:mm.")]
    public string HoraFim {get; set;} = string.Empty;
}

public class ExpedienteSaida
{
    public int DiaSemana {get; set;}
    public string NomeDia {get; set;} = string.Empty;
    public string HoraInicio {get; set;} = string.Empty;
    public string HoraFim {get; set;} = string.Empty;
}
