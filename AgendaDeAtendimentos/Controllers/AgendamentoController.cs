using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Controllers;

[ApiController]
[Route("api/agendamentos")]
[Authorize]
[Produces("application/json")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoService _service;

    public AgendamentoController(IAgendamentoService service)
    {
        _service = service;
    }

    private int ProfissionalLogadoId()
    {
        return int.Parse(User.FindFirst("id")!.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgendamentoSaida), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AgendamentoSaida>> Criar(AgendamentoEntrada entrada)
    {
        var agendamento = await _service.CriarAsync(ProfissionalLogadoId(), entrada);
        return StatusCode(StatusCodes.Status201Created, agendamento);
    }
}