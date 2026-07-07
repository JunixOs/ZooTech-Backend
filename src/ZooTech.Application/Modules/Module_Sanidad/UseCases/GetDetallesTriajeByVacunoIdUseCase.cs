using System.Globalization;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public interface IGetDetallesTriajeByVacunoIdInputPort
{
    Task<GetDetallesTriajeByVacunoIdOutput> HandleAsync(long vacunoId, CancellationToken cancellationToken = default);
}

public sealed record GetDetallesTriajeByVacunoIdOutput(
    IReadOnlyList<TriajeDetallePorVacunoItem> Items);

public sealed class GetDetallesTriajeByVacunoIdInteractor : IGetDetallesTriajeByVacunoIdInputPort
{
    private readonly ITriajeRepository _repository;

    public GetDetallesTriajeByVacunoIdInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDetallesTriajeByVacunoIdOutput> HandleAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetDetallesByVacunoIdAsync(vacunoId, cancellationToken);
        return new GetDetallesTriajeByVacunoIdOutput(items.ToList());
    }
}
