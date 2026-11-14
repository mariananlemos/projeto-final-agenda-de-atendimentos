using System.ComponentModel.DataAnnotations;

namespace AgendaDeAtendimentos.ProfissionalModels;

public class Profissional
{
    public int Id {get; set;}
    public string Nome {get; set;}
    public string Login {get; set;}
    public byte [] SenhaHash {get; set;}
    public byte [] SenhaSalt {get; set;}
}
