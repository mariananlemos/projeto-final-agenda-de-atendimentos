using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.ProfissionalModels;
using AgendaDeAtendimentos.ProfissionalService;

namespace AgendaDeAtendimentos.ProfissionalController;

[ApiController]
[Route("api/[controller]")]

public class ProfissionalController : ControllerBase
{
    private readonly IProfissionalService _service;
    public ProfissionalController(IProfissionalService service)
    {
        _service = service;
    }

    [HttpGet("{login}")]
    public async Task<IActionResult> BuscarProfissionalPorId(string login)
    {
        try
        {
            var profissional = await _service.BuscarProfissionalPorIdAsync(login);
            if(profissional == null)
            {
                return NotFound($"Profissional {login} não encontrado.");
            }
            return Ok(profissional);
        }
        catch(Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarProfissional(Profissional profissional)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var profissionalEncontrado = await _service.BuscarProfissionalPorIdAsync(profissional.Login);
        if(profissionalEncontrado != null)
        {
            return BadRequest($"Profissional {profissional.Login} já cadastrado.");
        }
        try
        {
            var profissionalCadastrado = await _service.CadastrarProfissionalAsync(profissional);
            return CreatedAtAction(nameof(BuscarProfissionalPorId), new {id = profissional.Id}, profissionalCadastrado);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex);
        }
    }
}