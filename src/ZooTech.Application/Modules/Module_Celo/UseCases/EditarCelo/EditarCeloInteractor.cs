using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;

public sealed class EditarCeloInteractor : IEditarCeloInputPort
{
    private readonly ICeloRepository _celoRepository;
    private readonly IValidator<EditarCeloCommand> _validator;

    public EditarCeloInteractor(ICeloRepository celoRepository, IValidator<EditarCeloCommand> validator)
    {
        _celoRepository = celoRepository;
        _validator = validator;
    }

    public async Task<EditarCeloOutput> HandleAsync(
        EditarCeloCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        var celo = await _celoRepository.GetByIdAsync(command.Id, cancellationToken);

        if (celo is null)
        {
            throw new NotFoundException($"No se encontró el registro de celo con ID {command.Id}.");
        }

        celo.Update(
            observaciones: command.Observaciones,
            caracteristicaCodes: command.CaracteristicaCodes,
            actorUsuarioId: null,
            utcNow: DateTime.UtcNow);

        var updated = await _celoRepository.UpdateAsync(celo, cancellationToken);

        return new EditarCeloOutput(
            Id: updated.Id,
            Observaciones: updated.Observaciones);
    }
}
