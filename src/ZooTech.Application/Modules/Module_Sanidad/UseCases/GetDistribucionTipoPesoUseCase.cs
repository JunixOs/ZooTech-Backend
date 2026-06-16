using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetDistribucionTipoPesoUseCase
{
    private readonly ITriajeRepository _repository;

    public GetDistribucionTipoPesoUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DTOs.Responses.TipoPesoCountItem>> ExecuteAsync(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var items = await _repository.GetDistribucionTipoPesoAsync(fechaInicio, fechaFin);

        return items.Select(d => new DTOs.Responses.TipoPesoCountItem
        {
            Code = d.Code,
            Nombre = d.Nombre,
            Cantidad = d.Cantidad
        });
    }
}
