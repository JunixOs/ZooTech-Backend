using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public sealed class UpdateCeloInteractor : IUpdateCeloInputPort
{
    private readonly ICeloRepository _celoRepository;
    private readonly IValidator<UpdateCeloCommand> _validator;

    public UpdateCeloInteractor(ICeloRepository celoRepository, IValidator<UpdateCeloCommand> validator)
    {
        _celoRepository = celoRepository;
        _validator = validator;
    }

    public async Task<UpdateCeloOutput> HandleAsync(
        UpdateCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var celo = await _celoRepository.GetByIdAsync(command.Id, cancellationToken);
        if (celo is null)
            throw new NotFoundException($"No se encontró el registro de celo con ID {command.Id}.");

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
