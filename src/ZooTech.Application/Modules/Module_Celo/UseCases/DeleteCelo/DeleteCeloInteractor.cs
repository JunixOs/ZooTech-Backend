using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public sealed class DeleteCeloInteractor : IDeleteCeloInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public DeleteCeloInteractor(IGanaderiaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmptyOutput> HandleAsync(
        DeleteCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.Celos;

        var celo = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (celo is null)
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Celo,
                $"No se encontró el registro de celo con ID {command.Id}."
            );

        celo.SoftDelete(
            motivoEliminacion: command.MotivoEliminacion,
            actorUsuarioId: null,
            utcNow: DateTime.UtcNow);

        await _unitOfWork.ExecuteInTransactionAsync(
            operation: ct => repository.UpdateAsync(celo, ct),
            cancellationToken: cancellationToken);

        return EmptyOutput.Value;
    }
}
