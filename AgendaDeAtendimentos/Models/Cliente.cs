namespace AgendaDeAtendimentos.Models;

public class Cliente
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public string Telefone {get; set;} = string.Empty;

    // cada cliente pertence a um profissional
    public int ProfissionalId {get; set;}
}
