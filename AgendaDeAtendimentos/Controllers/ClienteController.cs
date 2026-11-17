using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Services;

namespace AgendaDeAtendimentos.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;
    public ClienteController(IClienteService service)
    {
        _service = service;
    }

    // pega o id do profissional que está dentro do token JWT
    private int ProfissionalLogadoId()
    {
        return int.Parse(User.FindFirst("id")!.Value);
    }

    [HttpGet]
    public async Task<ActionResult<List<ClienteSaida>>> Listar()
    {
        var clientes = await _service.ListarAsync(ProfissionalLogadoId());
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteSaida>> BuscarPorId(int id)
    {
        var cliente = await _service.BuscarPorIdAsync(ProfissionalLogadoId(), id);
        if(cliente == null)
        {
            return NotFound(new ErroResposta { Mensagem = "Cliente não encontrado." });
        }
        return Ok(cliente);
    }

    // não precisa checar o ModelState aqui: o [ApiController] já devolve 400
    // no formato padrão (configurado no Program.cs) quando o DTO é inválido
    [HttpPost]
    public async Task<ActionResult<ClienteSaida>> Cadastrar(ClienteEntrada clienteEntrada)
    {
        var cliente = await _service.CadastrarAsync(ProfissionalLogadoId(), clienteEntrada);
        return CreatedAtAction(nameof(BuscarPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClienteSaida>> Atualizar(int id, ClienteEntrada clienteEntrada)
    {
        var cliente = await _service.AtualizarAsync(ProfissionalLogadoId(), id, clienteEntrada);
        if(cliente == null)
        {
            return NotFound(new ErroResposta { Mensagem = "Cliente não encontrado." });
        }
        return Ok(cliente);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var resultado = await _service.ExcluirAsync(ProfissionalLogadoId(), id);

        if(resultado == ResultadoExclusao.NaoEncontrado)
        {
            return NotFound(new ErroResposta { Mensagem = "Cliente não encontrado." });
        }
        if(resultado == ResultadoExclusao.PossuiAgendamentos)
        {
            return Conflict(new ErroResposta { Mensagem = "Não é possível excluir um cliente que possui agendamentos." });
        }
        return NoContent();
    }
}
