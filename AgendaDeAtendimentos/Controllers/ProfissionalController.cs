using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.ProfissionalService;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.AspNetCore.Authorization;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Account;

namespace AgendaDeAtendimentos.ProfissionalController;

[ApiController]
[Route("api/[controller]")]

public class ProfissionalController : ControllerBase
{
    private readonly IProfissionalService _service;
    private readonly IAuthenticate _authenticate;
    public ProfissionalController(IProfissionalService service, IAuthenticate authenticate)
    {
        _service = service;
        _authenticate = authenticate;
    }

    [HttpPost("Login-Credenciais")]
    //[Authorize]
    public async Task<IActionResult> BuscarProfissionalPorId(ProfissionalLogin profissionalLogin)
    {
        var profissional = await _authenticate.BuscarProfissionalPorLogin(profissionalLogin.Login);
        if(profissional == null)
        {
            return BadRequest(new {message = "Credenciais inválidas."});
        }
        var profissionalValido = await _authenticate.AuthenticateAsync(profissionalLogin.Login, profissionalLogin.Senha);
        if(!profissionalValido)
        {
            return BadRequest(new {message = "Credenciais inválidas."});
        }

        var token = _authenticate.GenerateToken(profissional.Id, profissional.Login.ToLower());
        return Ok(new {Nome = profissional.Nome, Token = token});
    }

    [HttpPost("Cadastro")]
    public async Task<IActionResult> CadastrarProfissional(ProfissionalCadastro profissionalCadastro)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var profissionalEncontrado = await _service.BuscarProfissionalPorIdAsync(profissionalCadastro.Login);
        if(profissionalEncontrado != null)
        {
            return BadRequest($"Profissional {profissionalCadastro.Login} já cadastrado.");
        }
        try
        {
            var profissionalCadastrado = await _service.CadastrarProfissionalAsync(profissionalCadastro);
            var token = _authenticate.GenerateToken(profissionalCadastrado.Id, profissionalCadastrado.Login.ToLower());
            return Ok(new{Nome = profissionalCadastrado.Nome, Token = token});// CreatedAtAction(nameof(BuscarProfissionalPorId), new {id = profissionalCadastro.Id}, profissionalCadastrado);
        }
        catch(ArgumentException ex)
        {
            return StatusCode(500, new
            {
                Message = ex.Message,
                InnerException = ex.InnerException?.Message
            });
        }
    }
}