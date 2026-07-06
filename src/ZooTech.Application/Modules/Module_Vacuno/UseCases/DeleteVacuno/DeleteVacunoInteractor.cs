using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public sealed class DeleteVacunoInteractor : IDeleteVacunoInputPort
{
    private readonly IVacunoRepository _repository;
    private readonly IValidator<DeleteVacunoCommand> _validator;

    public DeleteVacunoInteractor(IVacunoRepository repository, IValidator<DeleteVacunoCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task HandleAsync(long id, DeleteVacunoCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el vacuno solicitado.");

        try
        {
            existing.SoftDelete(command.MotivoEliminacion, null, DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new ConflictException(ex.Message);
        }

        _ = await _repository.UpdateAsync(existing, cancellationToken);
    }
}
