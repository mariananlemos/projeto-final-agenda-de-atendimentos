using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Models;
using AgendaDeAtendimentos.Repositories;

namespace AgendaDeAtendimentos.Services;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _repository;
    public ServicoService(IServicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ServicoSaida>> ListarAsync(int profissionalId)
    {
        var servicos = await _repository.ListarPorProfissionalAsync(profissionalId);
        return servicos.Select(ParaSaida).ToList();
    }

    public async Task<ServicoSaida?> BuscarPorIdAsync(int profissionalId, int id)
    {
        var servico = await _repository.BuscarPorIdAsync(profissionalId, id);
        return servico == null ? null : ParaSaida(servico);
    }

    public async Task<ServicoSaida> CadastrarAsync(int profissionalId, ServicoEntrada entrada)
    {
        var servico = new Servico
        {
            ProfissionalId = profissionalId,
            Nome = entrada.Nome.Trim(),
            DuracaoMinutos = entrada.DuracaoMinutos,
            Preco = entrada.Preco
        };

        await _repository.CadastrarAsync(servico);
        return ParaSaida(servico);
    }

    public async Task<ServicoSaida?> AtualizarAsync(int profissionalId, int id, ServicoEntrada entrada)
    {
        var servico = await _repository.BuscarPorIdAsync(profissionalId, id);
        if (servico == null) return null;

        servico.Nome = entrada.Nome.Trim();
        servico.DuracaoMinutos = entrada.DuracaoMinutos;
        servico.Preco = entrada.Preco;

        await _repository.AtualizarAsync(servico);
        return ParaSaida(servico);
    }

    public async Task<ResultadoExclusao> ExcluirAsync(int profissionalId, int id)
    {
        var servico = await _repository.BuscarPorIdAsync(profissionalId, id);
        if (servico == null) return ResultadoExclusao.NaoEncontrado;

        if (await _repository.PossuiAgendamentosAsync(id))
            return ResultadoExclusao.PossuiAgendamentos;

        await _repository.ExcluirAsync(servico);
        return ResultadoExclusao.Excluido;
    }

    private static ServicoSaida ParaSaida(Servico servico) => new()
    {
        Id = servico.Id,
        Nome = servico.Nome,
        DuracaoMinutos = servico.DuracaoMinutos,
        Preco = servico.Preco
    };
}