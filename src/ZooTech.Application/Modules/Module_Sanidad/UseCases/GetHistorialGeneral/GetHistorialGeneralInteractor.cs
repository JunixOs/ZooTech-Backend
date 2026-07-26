using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public sealed class GetHistorialGeneralInteractor : IGetHistorialGeneralInputPort
{
    private readonly ITriajeRepository _repository;

    public GetHistorialGeneralInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetHistorialGeneralOutput> HandleAsync(GetHistorialGeneralQuery query, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetHistorialGeneralAsync(query.FechaDesde, query.FechaHasta, cancellationToken);
        var output = items.Select(t => new HistorialTriajeItemOutput(t.Id, t.FechaHora, t.TipoPesoCode, t.PesoKg)).ToList();
        return new GetHistorialGeneralOutput(output);
    }
}
