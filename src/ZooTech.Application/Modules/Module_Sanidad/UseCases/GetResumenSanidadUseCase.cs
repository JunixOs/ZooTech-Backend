using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetResumenSanidadUseCase
{
    private readonly ITriajeRepository _repository;

    public GetResumenSanidadUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResumenSanidadResponse> ExecuteAsync(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var domain = await _repository.GetResumenAsync(fechaInicio, fechaFin);

        return new ResumenSanidadResponse
        {
            TotalTriajes = domain.TotalTriajes,
            PesoPromedioKg = domain.PesoPromedioKg,
            PesoMinimoKg = domain.PesoMinimoKg,
            PesoMaximoKg = domain.PesoMaximoKg,
            TotalVacunosEvaluados = domain.TotalVacunosEvaluados,
            DistribucionTipoPeso = domain.DistribucionTipoPeso.Select(d => new DTOs.Responses.TipoPesoCountItem
            {
                Code = d.Code,
                Nombre = d.Nombre,
                Cantidad = d.Cantidad
            }).ToList()
        };
    }
}
