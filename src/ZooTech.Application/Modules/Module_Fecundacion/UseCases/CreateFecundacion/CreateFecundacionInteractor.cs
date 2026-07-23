using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed class CreateFecundacionInteractor : ICreateFecundacionInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public CreateFecundacionInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        ITenantConfigurationProvider tenantConfigurationProvider
    )
    {
        _unitOfWork = unitOfWork;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<CreateFecundacionOutput> HandleAsync(
        CreateFecundacionCommand command,
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

        // Validar que el vacuno receptor exista
        if (!await repository.ExistsVacunoAsync(command.VacunoReceptorId, cancellationToken))
            throw new FecundacionVacunoNotFoundException();

        // Validar que no tenga otra fecundación activa con resultado 'Pendiente de confirmación'
        if (await repository.HasActiveFecundacionAsync(null, command.VacunoReceptorId, cancellationToken))
            throw new FecundacionPendingActiveException();

        // Validar que el celo exista (si se proporcionó)
        if (command.CeloRegistroId.HasValue && !await repository.ExistsCeloAsync(command.CeloRegistroId.Value, cancellationToken))
            throw new ConflictException(
                ScopeName.Application,
                ModuleName.Fecundacion,
                message: $"El registro de celo con ID {command.CeloRegistroId.Value} no existe."
            );

        // Validar que el vacuno donante exista (si no es macho externo y se proporcionó)
        if (!command.MachoExterno && command.VacunoDonanteId.HasValue)
        {
            if (!await repository.ExistsVacunoAsync(command.VacunoDonanteId.Value, cancellationToken))
                throw new ConflictException(
                    ScopeName.Application,
                    ModuleName.Fecundacion,
                    message: $"El vacuno donante con ID {command.VacunoDonanteId.Value} no existe."
                );
        }

        // Generar código único para la fecundación (Límite de la base de datos: 15 caracteres)
        var codigo = $"F{DateTime.UtcNow:yyMMddHHmmss}";
        if (await repository.ExistsCodigoAsync(codigo, cancellationToken))
            codigo = $"F{DateTime.UtcNow:yyMMddHHmmssff}";

        var utcNow = DateTime.UtcNow;

        var saved = await _unitOfWork.ExecuteInTransactionAsync(
            operation: async ct =>
            {
                var responsableId = await repository.GetOrCreateResponsableByNameAsync(
                    command.ResponsableName,
                    ct);
                var fecundacion = Fecundacion.CreateNew(
                    codigo: codigo,
                    tipoFecundacionCode: command.TipoFecundacionCode,
                    vacunoReceptorId: command.VacunoReceptorId,
                    celoRegistroId: command.CeloRegistroId,
                    fechaProcedimiento: command.FechaProcedimiento,
                    responsableId: responsableId,
                    resultadoCode: command.ResultadoCode,
                    observacionesVeterinarias: command.ObservacionesVeterinarias,
                    actorUsuarioId: command.CreatedById,
                    utcNow: utcNow,
                    machoExterno: command.MachoExterno,
                    machoExternoNombre: command.MachoExternoNombre,
                    vacunoDonanteId: command.VacunoDonanteId,
                    codigoSemen: command.CodigoSemen,
                    codigoEmbrion: command.CodigoEmbrion);

                return await repository.AddAsync(fecundacion, ct);
            },
            cancellationToken: cancellationToken,
            afterSave: async (domainBeforeSave, ct) =>
                await repository.GetByCodigoAsync(domainBeforeSave.Codigo, ct)
                ?? throw new ConflictException(
                    ScopeName.Application,
                    ModuleName.Fecundacion,
                    message: "No se pudo recuperar la fecundación persistida."));

        return new CreateFecundacionOutput(
            Id: saved.Id,
            Codigo: saved.Codigo,
            FechaProcedimiento: saved.FechaProcedimiento);
    }
}
