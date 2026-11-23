using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Controllers;

[ApiController]
[Route("api/servicos")]
[Authorize]
[Produces("application/json")]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _service;

    public ServicoController(IServicoService service)
    {
        _service = service;
    }

    private int ProfissionalLogadoId()
    {
        return int.Parse(User.FindFirst("id")!.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ServicoSaida>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ServicoSaida>>> Listar()
    {
        var servicos = await _service.ListarAsync(ProfissionalLogadoId());
        return Ok(servicos);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServicoSaida), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResposta), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ServicoSaida>> BuscarPorId(int id)
    {
        var servico = await _service.BuscarPorIdAsync(ProfissionalLogadoId(), id);
        if (servico == null)
        {
            return NotFound(new ErroResposta { Mensagem = "Serviço não encontrado." });
        }
        return Ok(servico);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServicoSaida), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ServicoSaida>> Cadastrar(ServicoEntrada servicoEntrada)
    {
        var servico = await _service.CadastrarAsync(ProfissionalLogadoId(), servicoEntrada);
        return CreatedAtAction(nameof(BuscarPorId), new { id = servico.Id }, servico);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ServicoSaida), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErroResposta), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ServicoSaida>> Atualizar(int id, ServicoEntrada servicoEntrada)
    {
        var servico = await _service.AtualizarAsync(ProfissionalLogadoId(), id, servicoEntrada);
        if (servico == null)
        {
            return NotFound(new ErroResposta { Mensagem = "Serviço não encontrado." });
        }
        return Ok(servico);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErroResposta), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErroResposta), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Excluir(int id)
    {
        var resultado = await _service.ExcluirAsync(ProfissionalLogadoId(), id);

        if (resultado == ResultadoExclusao.NaoEncontrado)
        {
            return NotFound(new ErroResposta { Mensagem = "Serviço não encontrado." });
        }
        if (resultado == ResultadoExclusao.PossuiAgendamentos)
        {
            return Conflict(new ErroResposta { Mensagem = "Não é possível excluir um serviço que possui agendamentos." });
        }
        return NoContent();
    }
}