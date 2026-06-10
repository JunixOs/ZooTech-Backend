using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public sealed class GetAllTipoPesosInteractor : IGetAllTipoPesosInputPort
{
    private readonly ITriajeRepository _repository;

    public GetAllTipoPesosInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTipoPesosOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _repository.GetAllTipoPesosAsync(cancellationToken);
        var items = tipos.Select(t => new TipoPesoItemOutput(t.Code, t.Nombre)).ToList();
        return new GetAllTipoPesosOutput(items);
    }
}
