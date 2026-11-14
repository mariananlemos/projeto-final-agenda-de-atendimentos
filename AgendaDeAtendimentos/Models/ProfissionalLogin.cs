using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.ProfissionalModels;

public class ProfissionalLogin
{
    [Required(ErrorMessage = "O login é um campo obrigatório.")]
    public string Login {get; set;}

    [Required(ErrorMessage = "A senha é um campo obrigatório")]
    public string Senha {get; set;} = string.Empty;
}