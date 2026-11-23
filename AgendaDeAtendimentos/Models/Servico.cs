using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaDeAtendimentos.Models;

public class Servico
{
    public int Id { get; set; }
    public int ProfissionalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }
    public decimal Preco { get; set; }
}

