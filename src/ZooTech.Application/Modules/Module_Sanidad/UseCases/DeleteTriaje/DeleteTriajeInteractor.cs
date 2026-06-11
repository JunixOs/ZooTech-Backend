using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public sealed class DeleteTriajeInteractor : IDeleteTriajeInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator<DeleteTriajeCommand> _validator;

    public DeleteTriajeInteractor(
        ITriajeRepository repository,
        IDateTimeProvider dateTimeProvider,
        IValidator<DeleteTriajeCommand> validator)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task HandleAsync(long id, DeleteTriajeCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var triaje = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el triaje solicitado.");

        triaje.SoftDelete(command.MotivoEliminacion, _dateTimeProvider.ServerNow);
        await _repository.UpdateAsync(triaje, cancellationToken);
    }
}
