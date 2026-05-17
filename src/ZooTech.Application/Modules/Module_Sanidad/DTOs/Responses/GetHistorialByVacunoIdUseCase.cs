using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetHistorialByVacunoIdUseCase
{
    private readonly ITriajeRepository _repository;

    public GetHistorialByVacunoIdUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TriajeHistorialResponse>> ExecuteAsync(long vacunoId)
    {
        var items = await _repository.GetHistorialByVacunoIdAsync(vacunoId);

        return items.Select(t => new TriajeHistorialResponse
        {
            Id = t.Id,
            FechaHora = t.FechaHora,
            TipoPesoCode = t.TipoPesoCode,
            PesoKg = t.PesoKg
        });
    }
}