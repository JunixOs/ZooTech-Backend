using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed class UpdateFecundacionInteractor : IUpdateFecundacionInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public UpdateFecundacionInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        ITenantConfigurationProvider tenantConfigurationProvider
    )
    {
        _unitOfWork = unitOfWork;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<UpdateFecundacionOutput> HandleAsync(
        UpdateFecundacionCommand command,
        CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.Fecundaciones;

        // Validar observaciones veterinarias contra la configuración del tenant
        if (!string.IsNullOrWhiteSpace(command.ObservacionesVeterinarias))
        {
            var maxLength = await _tenantConfigurationProvider.GetSettingAsync(
                Settings.Vacunos.VacunosFecundacionObservacionesMaxLength
            );
            if (command.ObservacionesVeterinarias.Length > maxLength)
            {
                throw new FecundacionValidationException(
                    $"Las observaciones veterinarias no pueden superar los {maxLength} caracteres.",
                    "OBSERVACIONES_MAX_LENGTH"
                );
            }
        }

        if (!await repository.ExistsVacunoAsync(command.VacunoReceptorId, cancellationToken))
            throw new FecundacionVacunoNotFoundException();

        if (await repository.HasActiveFecundacionAsync(command.Id, command.VacunoReceptorId, cancellationToken))
            throw new FecundacionPendingActiveException();

        var values = new FecundacionUpdateValues(
            command.TipoFecundacionCode,
            command.VacunoReceptorId,
            command.TipoDonante,
            command.VacunoDonanteId,
            command.ExternoDonanteNombre,
            command.FechaProcedimiento,
            command.ResponsableNombre,
            command.ResultadoCode,
            command.EstadoFecundacionCode,
            command.ObservacionesVeterinarias,
            command.CodigoSemen,
            command.CodigoEmbrion);

        var updated = await repository.UpdateAsync(command.Id, values, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        var detail = await repository.GetForEditAsync(command.Id, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        return new UpdateFecundacionOutput(
            detail.Id,
            detail.Codigo,
            detail.TipoFecundacionCode,
            detail.VacunoReceptorId,
            detail.VacunoReceptorCodigo,
            detail.VacunoReceptorNombre,
            detail.TipoDonante,
            detail.VacunoDonanteId,
            detail.VacunoDonanteCodigo,
            detail.VacunoDonanteNombre,
            detail.ExternoDonanteNombre,
            detail.FechaProcedimiento,
            detail.ResponsableNombre,
            updated.ResultadoCode,
            updated.EstadoFecundacionCode,
            detail.ObservacionesVeterinarias,
            detail.CodigoSemen,
            detail.CodigoEmbrion,
            detail.ActualizadoEn,
            updated.Warning);
    }
}
