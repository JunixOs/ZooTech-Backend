using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public sealed class GetAllTipoPesosInteractor : IGetAllTipoPesosInputPort
{
    private readonly ITipoPesoRepository _repository;

    public GetAllTipoPesosInteractor(ITipoPesoRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTipoPesosOutput> Handle(
        EmptyCommandQuery emptyCommand,
        CancellationToken cancellationToken = default
    )
    {
        var tipos = await _repository.GetAllAsync(cancellationToken);
        var items = tipos.Select(t => new TipoPesoItemOutput(t.Code, t.Nombre)).ToList();
        return new GetAllTipoPesosOutput(items);
    }
}
