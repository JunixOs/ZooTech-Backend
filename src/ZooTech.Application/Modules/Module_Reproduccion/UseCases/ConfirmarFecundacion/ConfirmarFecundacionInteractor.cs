using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;

namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public sealed class ConfirmarFecundacionInteractor : IConfirmarFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;
    private readonly IValidator<ConfirmarFecundacionCommand> _validator;

    public ConfirmarFecundacionInteractor(
        IFecundacionRepository repository,
        IValidator<ConfirmarFecundacionCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ConfirmarFecundacionOutput> HandleAsync(ConfirmarFecundacionCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var fecundacion = await _repository.GetByIdAsync(command.FecundacionId, cancellationToken);
        if (fecundacion is null)
        {
            throw new NotFoundException($"No se encontró la fecundación con ID {command.FecundacionId}.");
        }

        fecundacion.ConfirmResult(command.NuevoResultadoCode, command.CurrentUserId);

        await _repository.UpdateAsync(fecundacion, cancellationToken);

        return new ConfirmarFecundacionOutput(
            fecundacion.Id,
            fecundacion.ResultadoCode
        );
    }
}
