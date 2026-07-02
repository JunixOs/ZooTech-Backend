using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed class UpdateFecundacionEstadoInteractor : IUpdateFecundacionEstadoInputPort
{
    private readonly IFecundacionEstadoRepository repository;
    private readonly UpdateFecundacionEstadoValidator validator;
    private readonly FecundacionEstadoTransitionValidator transitionValidator;

    public UpdateFecundacionEstadoInteractor(
        IFecundacionEstadoRepository repository,
        UpdateFecundacionEstadoValidator validator,
        FecundacionEstadoTransitionValidator transitionValidator)
    {
        this.repository = repository;
        this.validator = validator;
        this.transitionValidator = transitionValidator;
    }

    public async Task<UpdateFecundacionEstadoOutput> HandleAsync(
        UpdateFecundacionEstadoCommand command,
        CancellationToken cancellationToken = default)
    {
        var nextEstado = validator.ValidateAndNormalize(command);

        var current = await repository.GetByFecundacionIdAsync(command.FecundacionId, cancellationToken)
            ?? throw new NotFoundException("No se encontro la fecundacion solicitada.");

        if (!current.EsHembra)
        {
            throw new ConflictException("El estado de fecundacion solo aplica a vacunos hembra.");
        }

        if (current.EstadoActual == FecundacionEstadoConstants.SinEstado)
        {
            throw new FecundacionEstadoValidationException(
                new Dictionary<string, string>
                {
                    ["estadoFecundacion"] = "La fecundacion no tiene un estado actual para actualizar."
                });
        }

        transitionValidator.ValidateTransition(current.EstadoActual, nextEstado);

        if (nextEstado == FecundacionEstadoConstants.EnProceso &&
            await repository.HasOtherActiveFecundacionAsync(
                current.VacunoId,
                command.FecundacionId,
                cancellationToken))
        {
            throw new ConflictException(
                "La hembra ya tiene una fecundacion activa en estado Pendiente o En proceso.");
        }

        var estadoCode = await repository.GetEstadoCodeByNameAsync(nextEstado, cancellationToken);
        if (estadoCode is null)
        {
            throw new ConflictException("El estado de fecundacion indicado no existe en la parametrizacion.");
        }

        await repository.UpdateEstadoAsync(
            command.FecundacionId,
            estadoCode,
            command.UpdatedBy!.Value,
            cancellationToken);

        var updated = await repository.GetByFecundacionIdAsync(command.FecundacionId, cancellationToken)
            ?? throw new NotFoundException("No se encontro la fecundacion solicitada.");

        return new UpdateFecundacionEstadoOutput(
            updated.VacunoId,
            updated.CodigoVacuno,
            updated.NombreVacuno,
            updated.EstadoActual,
            updated.DisponibleNuevaFecundacion,
            updated.UltimaActualizacion,
            updated.CodigoFecundacion,
            updated.TipoFecundacion,
            updated.ToroDonante,
            updated.Responsable,
            updated.FechaProcedimiento,
            updated.Resultado,
            updated.Observaciones);
    }
}
