using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed record UpdateTriajeOutput(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string? VacunoNombre,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed class UpdateTriajeInteractor : IUpdateTriajeInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public UpdateTriajeInteractor(IGanaderiaUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UpdateTriajeOutput> HandleAsync(UpdateTriajeCommand command, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.Triajes;

        var triaje = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Triaje,
                "No se encontró el triaje solicitado."
            );

        triaje.Update(
            command.TipoPesoCode,
            command.PesoKg,
            command.Observaciones,
            command.EncargadoUsuarioId,
            _dateTimeProvider.ServerNow);

        var updated = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(triaje, ct),
            cancellationToken);

        return ToOutput(updated);
    }

    private static UpdateTriajeOutput ToOutput(Triaje triaje) => new(
        triaje.Id,
        triaje.Codigo,
        triaje.FechaHora,
        triaje.VacunoId,
        triaje.VacunoNombre,
        triaje.TipoPesoCode,
        triaje.PesoKg,
        triaje.Observaciones,
        triaje.EstadoRegistroCode,
        triaje.EncargadoUsuarioId,
        triaje.CreatedAt,
        triaje.UpdatedAt);
}
