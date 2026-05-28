using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class UpdateTriajeUseCase
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTriajeUseCase(ITriajeRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TriajeResponse?> ExecuteAsync(long id, TriajeRequest request)
    {
        var triaje = await _repository.GetByIdAsync(id);

        if (triaje is null)
            return null;

        var now = _dateTimeProvider.ServerNow;

        //triaje.VacunoId = request.VacunoId;
        triaje.TipoPesoCode = request.TipoPesoCode;
        triaje.PesoKg = request.PesoKg;
        triaje.Observaciones = request.Observaciones;
        //triaje.EstadoRegistroCode = request.EstadoRegistroCode;
        //triaje.EncargadoUsuarioId = request.EncargadoUsuarioId;
        triaje.UpdatedAt = now;

        await _repository.UpdateAsync(triaje);

        return new TriajeResponse
        {
            Id = triaje.Id,
            Codigo = triaje.Codigo,
            FechaHora = triaje.FechaHora,
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