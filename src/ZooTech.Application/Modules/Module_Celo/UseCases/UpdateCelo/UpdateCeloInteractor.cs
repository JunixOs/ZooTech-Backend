using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public sealed class UpdateCeloInteractor : IUpdateCeloInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;

    public UpdateCeloInteractor(IGanaderiaUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCeloOutput> HandleAsync(
        UpdateCeloCommand command,
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

        celo.Update(
            observaciones: command.Observaciones,
            caracteristicaCodes: command.CaracteristicaCodes,
            actorUsuarioId: command.ActorUsuarioId,
            utcNow: DateTime.UtcNow);

        var updated = await _unitOfWork.ExecuteInTransactionAsync(
            operation: ct => repository.UpdateAsync(celo, ct),
            cancellationToken: cancellationToken);

        return new UpdateCeloOutput(
            Id: updated.Id,
            Observaciones: updated.Observaciones);
    }
}
