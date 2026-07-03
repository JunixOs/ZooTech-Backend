using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetHistorialGeneralUseCase
{
    private readonly ITriajeRepository _repository;

    public GetHistorialGeneralUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TriajeHistorialResponse>> ExecuteAsync(string? desde = null, string? hasta = null)
    {
        var items = await _repository.GetHistorialGeneralAsync(desde, hasta);

        return items.Select(t => new TriajeHistorialResponse
        {
            Id = t.Id,
            FechaHora = t.FechaHora,
            TipoPesoCode = t.TipoPesoCode,
            PesoKg = t.PesoKg
        });
    }
}
