using System.Globalization;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetDetallesTriajeByVacunoIdUseCase
{
    private readonly ITriajeRepository _repository;

    public GetDetallesTriajeByVacunoIdUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TriajeDetallePorVacunoResponse>> ExecuteAsync(long vacunoId)
    {
        var items = await _repository.GetDetallesByVacunoIdAsync(vacunoId);

        return items.Select(item => new TriajeDetallePorVacunoResponse
        {
            CodigoRegistro = item.CodigoRegistro,
            Fecha = item.FechaHora.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Hora = item.FechaHora.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            TipoPesoMedido = item.TipoPesoMedido,
            PesoKg = item.PesoKg,
            Observaciones = item.Observaciones
        });
    }
}
