using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetAllTriajesUseCase
{
    private readonly ITriajeRepository _repository;

    public GetAllTriajesUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TriajeResponse>> ExecuteAsync()
    {
        var triajes = await _repository.GetAllAsync();

        return triajes.Select(t => new TriajeResponse
        {
            Id = t.Id,
            Codigo = t.Codigo,
            FechaHora = t.FechaHora,
            VacunoId = t.VacunoId,
            TipoPesoCode = t.TipoPesoCode,
            PesoKg = t.PesoKg,
            Observaciones = t.Observaciones,
            EstadoRegistroCode = t.EstadoRegistroCode,
            EncargadoUsuarioId = t.EncargadoUsuarioId,
            CreatedAt = t.CreatedAt
        });
    }
}