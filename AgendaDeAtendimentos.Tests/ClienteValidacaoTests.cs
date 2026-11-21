using System.ComponentModel.DataAnnotations;
using AgendaDeAtendimentos.DTO;

namespace AgendaDeAtendimentos.Tests;

public class ClienteValidacaoTests
{
    private static List<ValidationResult> Validar(ClienteEntrada cliente)
    {
        var erros = new List<ValidationResult>();
        Validator.TryValidateObject(cliente, new ValidationContext(cliente), erros, true);
        return erros;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A")]
    public void Nome_Invalido_Curto_Ou_Vazio_Deve_Dar_Erro(string nome)
    {
        var cliente = new ClienteEntrada { Nome = nome, Telefone = "11999999999" };

        var erros = Validar(cliente);

        Assert.Contains(erros, e => e.MemberNames.Contains("Nome"));
    }

    [Fact]
    public void Nome_Com_Mais_De_100_Caracteres_Deve_Dar_Erro()
    {
        var cliente = new ClienteEntrada { Nome = new string('a', 101), Telefone = "11999999999" };

        var erros = Validar(cliente);

        Assert.Contains(erros, e => e.MemberNames.Contains("Nome"));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(100)]
    public void Nome_Com_Tamanho_No_Limite_Deve_Ser_Valido(int tamanho)
    {
        var cliente = new ClienteEntrada { Nome = new string('a', tamanho), Telefone = "11999999999" };

        var erros = Validar(cliente);

        Assert.Empty(erros);
    }
}
