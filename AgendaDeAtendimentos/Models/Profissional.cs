using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.Models;

public class Profissional
{
    public int Id {get; set;}
    public string Nome {get; set;}
    public string Login {get; set;}

    [Required(ErrorMessage = "A senha é um campo obrigatório")]
    [MinLength(8, ErrorMessage = "A senha precisar ter mais de 8 caracteres")]
    private string Senha {get; set;}
}
