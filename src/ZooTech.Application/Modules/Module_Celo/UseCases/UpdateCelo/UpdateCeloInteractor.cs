using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public sealed class UpdateCeloInteractor : IUpdateCeloInputPort
{
    private readonly ICeloRepository _celoRepository;

    public UpdateCeloInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<UpdateCeloOutput> Handle(
        UpdateCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        var celo = await _celoRepository.GetByIdAsync(command.Id, cancellationToken);
        if (celo is null)
            throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Celo,
                $"No se encontró el registro de celo con ID {command.Id}."
            );

        celo.Update(
            observaciones: command.Observaciones,
            caracteristicaCodes: command.CaracteristicaCodes,
            actorUsuarioId: null,
            utcNow: DateTime.UtcNow);

        var updated = await _celoRepository.UpdateAsync(celo, cancellationToken);

        return new UpdateCeloOutput(
            Id: updated.Id,
            Observaciones: updated.Observaciones);
    }
}
