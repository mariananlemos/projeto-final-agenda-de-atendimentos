namespace AgendaDeAtendimentos.Exceptions;

// erro de regra de negócio ligado a um campo específico (vira 400 no controller)
public class ValidacaoException : Exception
{
    public string Campo {get; set;}

    public ValidacaoException(string campo, string mensagem) : base(mensagem)
    {
        Campo = campo;
    }
}
