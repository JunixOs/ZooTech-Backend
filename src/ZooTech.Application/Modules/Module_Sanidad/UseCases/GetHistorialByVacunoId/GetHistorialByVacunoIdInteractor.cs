using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public sealed class GetHistorialByVacunoIdInteractor : IGetHistorialByVacunoIdInputPort
{
    private readonly ITriajeRepository _repository;

    public GetHistorialByVacunoIdInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetHistorialByVacunoIdOutput> HandleAsync(long vacunoId, string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetHistorialByVacunoIdAsync(vacunoId, fechaDesde, fechaHasta, cancellationToken);
        var output = items.Select(t => new HistorialTriajeItemOutput(t.Id, t.FechaHora, t.TipoPesoCode, t.PesoKg)).ToList();
        return new GetHistorialByVacunoIdOutput(output);
    }
}
