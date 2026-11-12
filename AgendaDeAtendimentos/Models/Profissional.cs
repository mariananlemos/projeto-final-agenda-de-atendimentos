using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.ProfissionalModels;

public class Profissional
{
    public int Id {get; set;}
    public string Nome {get; set;}
    public string Login {get; set;}

    [Required(ErrorMessage = "A senha é um campo obrigatório")]
    [MinLength(8, ErrorMessage = "A senha precisar ter mais de 8 caracteres")]
    public string Senha {get; set;}
}
