using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;

namespace AgendaDeAtendimentos.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ClienteSaida>> ListarAsync(int profissionalId)
    {
        var clientes = await _repository.ListarPorProfissionalAsync(profissionalId);
        return clientes.Select(c => ParaSaida(c)).ToList();
    }

    public async Task<ClienteSaida?> BuscarPorIdAsync(int profissionalId, int id)
    {
        var cliente = await _repository.BuscarPorIdAsync(profissionalId, id);
        if(cliente == null)
        {
            return null;
        }
        return ParaSaida(cliente);
    }

    public async Task<ClienteSaida> CadastrarAsync(int profissionalId, ClienteEntrada clienteEntrada)
    {
        var cliente = new Cliente
        {
            Nome = clienteEntrada.Nome,
            Telefone = clienteEntrada.Telefone, // salvo do jeito que veio
            ProfissionalId = profissionalId
        };

        var clienteCadastrado = await _repository.CadastrarAsync(cliente);
        return ParaSaida(clienteCadastrado);
    }

    public async Task<ClienteSaida?> AtualizarAsync(int profissionalId, int id, ClienteEntrada clienteEntrada)
    {
        var cliente = await _repository.BuscarPorIdAsync(profissionalId, id);
        if(cliente == null)
        {
            return null;
        }

        cliente.Nome = clienteEntrada.Nome;
        cliente.Telefone = clienteEntrada.Telefone;
        await _repository.AtualizarAsync(cliente);
        return ParaSaida(cliente);
    }

    public async Task<ResultadoExclusao> ExcluirAsync(int profissionalId, int id)
    {
        var cliente = await _repository.BuscarPorIdAsync(profissionalId, id);
        if(cliente == null)
        {
            return ResultadoExclusao.NaoEncontrado;
        }

        // regra: não pode excluir cliente que já tem agendamento
        if(await _repository.PossuiAgendamentosAsync(cliente.Id))
        {
            return ResultadoExclusao.PossuiAgendamentos;
        }

        await _repository.ExcluirAsync(cliente);
        return ResultadoExclusao.Excluido;
    }

    private static ClienteSaida ParaSaida(Cliente cliente)
    {
        return new ClienteSaida
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Telefone = cliente.Telefone
        };
    }
}
