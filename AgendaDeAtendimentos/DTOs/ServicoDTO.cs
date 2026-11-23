using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.DTO;

public class ServicoEntrada : IValidatableObject
{
    [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A duração é obrigatória.")]
    public int DuracaoMinutos { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório.")]
    [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo.")]
    public decimal Preco { get; set; }

    // Valida regra específica de intervalos de 5 minutos e aponta exatamente o campo "duracaoMinutos"
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DuracaoMinutos < 5 || DuracaoMinutos % 5 != 0)
        {
            yield return new ValidationResult(
                "A duração deve ser de pelo menos 5 minutos e em múltiplos de 5.",
                new[] { nameof(DuracaoMinutos) }
            );
        }
    }
}

public class ServicoSaida
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }
    public decimal Preco { get; set; }
}