using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetGeneralTriajesReportUseCase
{
    private readonly ITriajeRepository _repository;

    public GetGeneralTriajesReportUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TriajeResponse>> ExecuteAsync(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return Enumerable.Empty<TriajeResponse>();
        }

        var triajes = await _repository.GetGeneralReportAsync(startDate, endDate);

        return triajes.Select(t => new TriajeResponse
        {
            Id = t.Id,
            Codigo = t.Codigo,
            FechaHora = t.FechaHora,
            VacunoId = t.VacunoId,
            VacunoNombre = t.VacunoNombre,
            TipoPesoCode = t.TipoPesoCode,
            PesoKg = t.PesoKg,
            Observaciones = t.Observaciones,
            EstadoRegistroCode = t.EstadoRegistroCode,
            EncargadoUsuarioId = t.EncargadoUsuarioId,
            CreatedAt = t.CreatedAt
        });
    }
}
