using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.DTO;

public class AgendamentoEntrada
{
    [Required(ErrorMessage = "O cliente é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Cliente inválido.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O serviço é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Serviço inválido.")]
    public int ServicoId { get; set; }

    [Required(ErrorMessage = "A data e hora de início são obrigatórias.")]
    public DateTime DataHoraInicio { get; set; }
}

public class AgendamentoSaida
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public DateTime DataHoraInicio { get; set; }
    public DateTime DataHoraFim { get; set; }
    public decimal PrecoCobrado { get; set; }
    public string Status { get; set; } = string.Empty;
}