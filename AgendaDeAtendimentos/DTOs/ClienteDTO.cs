using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.DTO;

// usado no POST e no PUT
public class ClienteEntrada
{
    [Required(ErrorMessage = "O nome é um campo obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome precisa ter entre 2 e 100 caracteres.")]
    public string Nome {get; set;} = string.Empty;

    // o telefone é salvo exatamente como o usuário digitou
    [Required(ErrorMessage = "O telefone é um campo obrigatório.")]
    [StringLength(20, ErrorMessage = "O telefone pode ter no máximo 20 caracteres.")]
    public string Telefone {get; set;} = string.Empty;
}

// o que a API devolve
public class ClienteSaida
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public string Telefone {get; set;} = string.Empty;
}
