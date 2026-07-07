using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed class UpdateTriajeInteractor : IUpdateTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public UpdateTriajeInteractor(ITriajeRepository repository, IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<UpdateTriajeOutput> Handle(UpdateTriajeCommand command, CancellationToken cancellationToken = default)
    {
        var triaje = await _repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Triaje,
                "No se encontró el triaje solicitado."
            );

        triaje.Update(
            command.VacunoId,
            command.TipoPesoCode,
            command.PesoKg,
            command.Observaciones,
            command.EstadoRegistroCode,
            command.EncargadoUsuarioId,
            _dateTimeProvider.ServerNow);

        await _repository.UpdateAsync(triaje, cancellationToken);

        return new UpdateTriajeOutput(
            triaje.Id, triaje.Codigo, triaje.FechaHora, triaje.VacunoId,
            triaje.TipoPesoCode, triaje.PesoKg, triaje.Observaciones,
            triaje.EstadoRegistroCode, triaje.EncargadoUsuarioId, triaje.CreatedAt);
    }
}
