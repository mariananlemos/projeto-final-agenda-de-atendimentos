using System.Text;
using AgendaDeAtendimentos.Account;
using AgendaDeAtendimentos.Data;
using System.Security.Claims;
using AgendaDeAtendimentos.ProfissionalModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AgendaDeAtendimentos.AuthenticateService
{
    public class AuthenticateService : IAuthenticate
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticateService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string GenerateToken(int id, string login)
        {
            var claims = new[]
            {
                new Claim("id", id.ToString()),
                new Claim("login", login.ToLower()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Profissional> BuscarProfissionalPorLogin(string login)
        {
            return await _context.Profissionals.FirstOrDefaultAsync(p => p.Login.ToLower() == login.ToLower());
        }

        public async Task<bool> AuthenticateAsync(string login, string senha)
        {
            var profissional = await _context.Profissionals.FirstOrDefaultAsync(p => p.Login.ToLower() == login.ToLower());
            if(profissional == null)
            {
                return false;
            }

            using var hmac = new HMACSHA512(profissional.SenhaSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
            for(int i = 0; i < computeHash.Length; i++)
            {
                if(computeHash[i] != profissional.SenhaHash[i])
                {
                    return false;
                }
            }
            return true;
        }

        public Task<bool> ProfissionalEncontrado(string login)
        {
            throw new NotImplementedException();
        }
    }
}