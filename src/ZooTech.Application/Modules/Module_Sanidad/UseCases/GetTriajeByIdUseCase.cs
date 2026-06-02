using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetTriajeByIdUseCase
{
    private readonly ITriajeRepository _repository;

    public GetTriajeByIdUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<TriajeResponse?> ExecuteAsync(long id)
    {
        var triaje = await _repository.GetByIdAsync(id);

        if (triaje is null)
            return null;

        return new TriajeResponse
        {
            Id = triaje.Id,
            Codigo = triaje.Codigo,
            FechaHora = triaje.FechaHora,
            VacunoNombre= triaje.VacunoNombre,
            VacunoId = triaje.VacunoId,
            TipoPesoCode = triaje.TipoPesoCode,
            PesoKg = triaje.PesoKg,
            Observaciones = triaje.Observaciones,
            EstadoRegistroCode = triaje.EstadoRegistroCode,
            EncargadoUsuarioId = triaje.EncargadoUsuarioId,
            CreatedAt = triaje.CreatedAt
        };
    }
}