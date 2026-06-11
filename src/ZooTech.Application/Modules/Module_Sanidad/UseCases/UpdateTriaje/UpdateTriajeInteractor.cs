using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed class UpdateTriajeInteractor : IUpdateTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<UpdateTriajeCommand> _validator;

    public UpdateTriajeInteractor(ITriajeRepository repository, IDateTimeProvider dateTimeProvider, IValidator<UpdateTriajeCommand> validator)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task<UpdateTriajeOutput> HandleAsync(long id, UpdateTriajeCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var triaje = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el triaje solicitado.");

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
