using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public sealed class DeleteCeloInteractor : IDeleteCeloInputPort
{
    private readonly ICeloRepository _celoRepository;
    private readonly IValidator<DeleteCeloCommand> _validator;

    public DeleteCeloInteractor(ICeloRepository celoRepository, IValidator<DeleteCeloCommand> validator)
    {
        _celoRepository = celoRepository;
        _validator = validator;
    }

    public async Task HandleAsync(
        DeleteCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var celo = await _celoRepository.GetByIdAsync(command.Id, cancellationToken);
        if (celo is null)
            throw new NotFoundException($"No se encontró el registro de celo con ID {command.Id}.");

        celo.SoftDelete(
            motivoEliminacion: command.MotivoEliminacion,
            actorUsuarioId: null,
            utcNow: DateTime.UtcNow);

        await _celoRepository.UpdateAsync(celo, cancellationToken);
    }
}
