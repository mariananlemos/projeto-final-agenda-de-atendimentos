using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.DTO;

public class ProfissionalCadastro
{
    public int Id {get; set;}
    
    [Required(ErrorMessage = "O nome é um campo obrigatório.")]
    public string Nome {get; set;}

    [Required(ErrorMessage = "O login é um campo obrigatório.")]
    public string Login {get; set;}

    [Required(ErrorMessage = "A senha é um campo obrigatório")]
    [MinLength(8, ErrorMessage = "A senha precisar ter mais de 8 caracteres")]
    public string Senha {get; set;} = string.Empty;

}