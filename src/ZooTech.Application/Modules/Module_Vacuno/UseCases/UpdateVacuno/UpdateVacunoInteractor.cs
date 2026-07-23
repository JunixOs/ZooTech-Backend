using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed class UpdateVacunoInteractor : IUpdateVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly IVacunoReferenceResolver _referenceResolver;

    public UpdateVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        ITenantConfigurationProvider tenantConfigurationProvider,
        IVacunoReferenceResolver referenceResolver)
    {
        _unitOfWork = unitOfWork;
        _tenantConfigurationProvider = tenantConfigurationProvider;
        _referenceResolver = referenceResolver;
    }

    public async Task<UpdateVacunoOutput> HandleAsync(UpdateVacunoCommand command, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Vacunos;
        var existing = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe el vacuno con el ID {command.Id}.");

        if (existing.FechaNacimiento != command.FechaNacimiento)
        {
            throw new ImmutableFieldException("No se permite editar la fecha de nacimiento.");
        }

        if (existing.TipoAdquisicionCode != command.TipoAdquisicionCode)
        {
            throw new ImmutableFieldException("No se permite editar el tipo de adquisicion.");
        }

        var validationSettings = await VacunoTenantValidationSettings.LoadAsync(_tenantConfigurationProvider);
        ValidateCommand(command, validationSettings);
        var referenceResolution = await _referenceResolver.ResolveAsync(
            ToReferenceData(command, existing.Codigo),
            cancellationToken);

        if (!referenceResolution.Success)
        {
            var error = referenceResolution.Error!;
            throw VacunoValidationErrorDetails.CreateException(
                error.Field ?? "formulario",
                "VACUNO-REFERENCE-INVALID",
                error.Message);
        }

        var updated = await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var granjaId = await ResolveGranjaIdForMutationAsync(referenceResolution, ct);
                try
                {
                    existing.Update(
                        command.Nombre,
                        command.FechaNacimiento,
                        command.TipoAdquisicionCode,
                        command.RazaCode,
                        command.ColorCode,
                        command.SexoCode,
                        referenceResolution.PadreId,
                        referenceResolution.MadreId,
                        granjaId,
                        command.Observaciones,
                        validationSettings.ToDomainLimits(),
                        null,
                        DateTime.UtcNow);
                }
                catch (ArgumentException ex)
                {
                    throw VacunoValidationErrorDetails.CreateException(
                        "formulario",
                        "VACUNO-DOMAIN-INVALID",
                        ex.Message);
                }

                return await repository.UpdateAsync(existing, command.PrecioCompra, command.AptoPara, ct);
            },
            cancellationToken);

        return new UpdateVacunoOutput(VacunoAppMapper.ToOutput(updated));
    }

    private async Task<long> ResolveGranjaIdForMutationAsync(
        VacunoReferenceResolution resolution,
        CancellationToken cancellationToken)
    {
        if (resolution.GranjaId.HasValue)
        {
            return resolution.GranjaId.Value;
        }

        var granja = resolution.GranjaToCreate
            ?? throw VacunoValidationErrorDetails.CreateException(
                "granjaId",
                "VACUNO-GRANJA-REQUIRED",
                "Debe seleccionar o registrar una granja valida.");

        return await _unitOfWork.Vacunos.EnsureGranjaAsync(
            granja.Nombre,
            granja.CodigoDistrito,
            cancellationToken);
    }

    private static VacunoReferenceData ToReferenceData(UpdateVacunoCommand command, string ownCodigo)
        => new(
            ownCodigo,
            command.CodigoPadre,
            command.CodigoMadre,
            command.GranjaId,
            command.Granja,
            command.CodigoDistrito);

    private static void ValidateCommand(UpdateVacunoCommand command, VacunoTenantValidationSettings settings)
    {
        settings.ValidateInput(command.Nombre, "nombre");
        settings.ValidateInput(command.TipoAdquisicionCode, "tipoAdquisicionCode");
        settings.ValidateInput(command.RazaCode, "razaCode");
        settings.ValidateInput(command.ColorCode, "colorCode");
        settings.ValidateInput(command.SexoCode, "sexoCode");
        settings.ValidateObservaciones(command.Observaciones);
    }
}
