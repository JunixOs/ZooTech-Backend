using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class CreateTriajeUseCase
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTriajeUseCase(ITriajeRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TriajeResponse> ExecuteAsync(TriajeRequest request)
    {
        var codigo = await _repository.GenerateCodigoAsync();
        var now = _dateTimeProvider.ServerNow;

        var triaje = new Triaje
        {
            Codigo = codigo,
            FechaHora = now,
            VacunoId = request.VacunoId,
            TipoPesoCode = request.TipoPesoCode,
            PesoKg = request.PesoKg,
            Observaciones = request.Observaciones,
            EstadoRegistroCode = request.EstadoRegistroCode,
            EncargadoUsuarioId = request.EncargadoUsuarioId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.AddAsync(triaje);

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