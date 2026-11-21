using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Exceptions;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Controllers;

[ApiController]
[Route("api/expediente")]
[Authorize]
public class ExpedienteController : ControllerBase
{
    private readonly IExpedienteService _service;
    public ExpedienteController(IExpedienteService service)
    {
        _service = service;
    }

    private int ProfissionalLogadoId()
    {
        return int.Parse(User.FindFirst("id")!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<List<ExpedienteSaida>>> Listar()
    {
        var expedientes = await _service.ListarAsync(ProfissionalLogadoId());
        return Ok(expedientes);
    }

    // {dia}: 0 = domingo, 1 = segunda ... 6 = sábado
    [HttpPut("{dia}")]
    public async Task<ActionResult<ExpedienteSaida>> Salvar(int dia, ExpedienteEntrada expedienteEntrada)
    {
        try
        {
            var expediente = await _service.SalvarAsync(ProfissionalLogadoId(), dia, expedienteEntrada);
            return Ok(expediente);
        }
        catch(ValidacaoException ex)
        {
            return BadRequest(ErroResposta.DoCampo(ex.Campo, ex.Message));
        }
    }

    [HttpDelete("{dia}")]
    public async Task<IActionResult> Excluir(int dia)
    {
        try
        {
            var excluido = await _service.ExcluirAsync(ProfissionalLogadoId(), dia);
            if(!excluido)
            {
                return NotFound(new ErroResposta { Mensagem = "Não existe expediente cadastrado para esse dia." });
            }
            return NoContent();
        }
        catch(ValidacaoException ex)
        {
            return BadRequest(ErroResposta.DoCampo(ex.Campo, ex.Message));
        }
    }
}
