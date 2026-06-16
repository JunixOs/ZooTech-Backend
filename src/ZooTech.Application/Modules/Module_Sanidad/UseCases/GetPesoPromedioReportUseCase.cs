using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetPesoPromedioReportUseCase
{
    private readonly ITriajeRepository _repository;

    public GetPesoPromedioReportUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DTOs.Responses.PesoPromedioItem>> ExecuteAsync(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var items = await _repository.GetPesoPromedioPorPeriodoAsync(fechaInicio, fechaFin);

        return items.Select(i => new DTOs.Responses.PesoPromedioItem
        {
            Fecha = i.Fecha,
            PesoPromedio = i.PesoPromedio,
            CantidadRegistros = i.CantidadRegistros
        });
    }
}
