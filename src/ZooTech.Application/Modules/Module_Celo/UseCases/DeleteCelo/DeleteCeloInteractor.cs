using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public sealed class DeleteCeloInteractor : IDeleteCeloInputPort
{
    private readonly ICeloRepository _celoRepository;

    public DeleteCeloInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<EmptyOutput> HandleAsync(
        DeleteCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        var celo = await _celoRepository.GetByIdAsync(command.Id, cancellationToken);
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

        await _celoRepository.UpdateAsync(celo, cancellationToken);
    
        return EmptyOutput.Value;
    }
}
