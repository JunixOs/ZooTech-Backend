using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Module_Sanidad.Rules;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class CreateTriajeUseCase
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SanidadSettings _settings;

    public CreateTriajeUseCase(ITriajeRepository repository, IDateTimeProvider dateTimeProvider, SanidadSettings settings)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _settings = settings;
    }

    public async Task<TriajeResponse> ExecuteAsync(TriajeRequest request)
    {
        TriajeRule.ValidarVacunoId(request.VacunoId);
        TriajeRule.ValidarTipoPesoCode(request.TipoPesoCode);
        TriajeRule.ValidarPesoKg(request.PesoKg);
        TriajeRule.ValidarObservaciones(request.Observaciones);

        var codigo = await _repository.GenerateCodigoAsync();
        var now = _dateTimeProvider.ServerNow;

        // Evitar conflictos con la restricción ck_triaje_fecha por desincronización de relojes (clock skew).
        // Si la fecha/hora es futura o demasiado cercana al tiempo del servidor, la limitamos a N minutos en el pasado.
        var fechaHora = request.FechaHora;
        var toleranciaMinutos = _settings.ToleranciaRelojMinutos;
        if (fechaHora > now.AddMinutes(-toleranciaMinutos))
        {
            fechaHora = now.AddMinutes(-toleranciaMinutos);
        }

        var triaje = new Triaje
        {
            Codigo = codigo,
            FechaHora = fechaHora,
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