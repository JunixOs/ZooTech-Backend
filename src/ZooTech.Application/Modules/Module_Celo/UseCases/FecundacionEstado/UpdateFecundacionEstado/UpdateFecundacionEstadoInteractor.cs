using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed class UpdateFecundacionEstadoInteractor : IUpdateFecundacionEstadoInputPort
{
    private readonly IFecundacionEstadoRepository repository;
    private readonly FecundacionEstadoTransitionValidator transitionValidator;

    public UpdateFecundacionEstadoInteractor(
        IFecundacionEstadoRepository repository,
        FecundacionEstadoTransitionValidator transitionValidator)
    {
        this.repository = repository;
        this.transitionValidator = transitionValidator;
    }

    public async Task<UpdateFecundacionEstadoOutput> HandleAsync(
        UpdateFecundacionEstadoCommand command,
        CancellationToken cancellationToken = default)
    {
        var estadoFecundacionNormalized = command.EstadoFecundacion?.Trim() ?? string.Empty;

        var current = await repository.GetByFecundacionIdAsync(command.FecundacionId, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                "No se encontro la fecundacion solicitada."
            );

        if (!current.EsHembra)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message:  "El estado de fecundacion solo aplica a vacunos hembra."
            );
        }

        if (current.EstadoActual == FecundacionEstadoConstants.SinEstado)
        {
            throw new FecundacionEstadoValidationException(
                new List<string>
                {
                    "FECUNDACION-UPDATE-FECUNDACION_ESTADO-INVALID"
                },
                "La fecundacion no tiene un estado actual para actualizar."
            );
        }

        transitionValidator.ValidateTransition(current.EstadoActual, estadoFecundacionNormalized);

        if (FecundacionEstadoConstants.IsActive(estadoFecundacionNormalized) &&
            await repository.HasOtherActiveFecundacionAsync(
                current.VacunoId,
                command.FecundacionId,
                cancellationToken))
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message: "La hembra ya tiene una fecundacion activa en estado Pendiente o En proceso."
            );
        }

        var estadoCode = await repository.GetEstadoCodeByNameAsync(estadoFecundacionNormalized, cancellationToken);
        if (estadoCode is null)
        {
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message: "El estado de fecundacion indicado no existe en la parametrizacion."
            );
        }

        await repository.UpdateEstadoAsync(
            command.FecundacionId,
            estadoCode,
            command.UpdatedBy!.Value,
            cancellationToken);

        var updated = await repository.GetByFecundacionIdAsync(command.FecundacionId, cancellationToken)
            ?? throw new NotFoundException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                "No se encontro la fecundacion solicitada."
            );

        return new UpdateFecundacionEstadoOutput(
            updated.VacunoId,
            updated.FecundacionId,
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
