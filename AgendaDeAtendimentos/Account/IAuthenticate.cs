using System.Collections.Generic;
using System.Text;
using AgendaDeAtendimentos.ProfissionalModels;

namespace AgendaDeAtendimentos.Account
{
    public interface IAuthenticate
    {
        string GenerateToken(int id, string login);
        Task<Profissional> BuscarProfissionalPorLogin(string login);
        Task<bool> ProfissionalEncontrado(string login);
        Task<bool> AuthenticateAsync(string login, string senha);
    }
}