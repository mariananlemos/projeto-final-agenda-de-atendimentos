namespace AgendaDeAtendimentos.DTO;

// formato padrão dos erros da API
// exemplo: { "mensagem": "Dados inválidos.", "erros": [ { "campo": "nome", "mensagem": "..." } ] }
public class ErroResposta
{
    public string Mensagem {get; set;} = string.Empty;
    public List<ErroCampo> Erros {get; set;} = new List<ErroCampo>();

    public static ErroResposta DoCampo(string campo, string mensagem)
    {
        var resposta = new ErroResposta { Mensagem = "Dados inválidos." };
        resposta.Erros.Add(new ErroCampo { Campo = campo, Mensagem = mensagem });
        return resposta;
    }
}

public class ErroCampo
{
    public string Campo {get; set;} = string.Empty;
    public string Mensagem {get; set;} = string.Empty;
}
