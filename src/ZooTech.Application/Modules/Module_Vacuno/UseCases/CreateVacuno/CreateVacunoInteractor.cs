using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed class CreateVacunoInteractor : ICreateVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IAppCacheService _cache;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly IVacunoReferenceResolver _referenceResolver;

    public CreateVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IAppCacheService cache,
        ITenantConfigurationProvider tenantConfigurationProvider,
        IVacunoReferenceResolver referenceResolver)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _tenantConfigurationProvider = tenantConfigurationProvider;
        _referenceResolver = referenceResolver;
    }

    public async Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken)
    {
        var validationSettings = await VacunoTenantValidationSettings.LoadAsync(_tenantConfigurationProvider);
        ValidateCommand(command, validationSettings);
        var repository = _unitOfWork.Vacunos;
        var referenceResolution = await _referenceResolver.ResolveAsync(
            ToReferenceData(command),
            cancellationToken);

        if (!referenceResolution.Success)
        {
            var error = referenceResolution.Error!;
            throw VacunoValidationErrorDetails.CreateException(
                error.Field ?? "formulario",
                "VACUNO-REFERENCE-INVALID",
                error.Message);
        }

        if (await repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
            throw new VacunoAlreadyExistsException("Ya existe un vacuno con ese codigo o datos repetidos.");

        var saved = await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var granjaId = await ResolveGranjaIdForMutationAsync(referenceResolution, ct);
                var vacuno = CreateDomainVacuno(command, referenceResolution, validationSettings, granjaId);
                return await repository.AddAsync(vacuno, command.PrecioCompra, command.AptoPara, ct);
            },
            cancellationToken,
            async (_, ct) => await repository.GetByCodigoAsync(command.Codigo, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar el vacuno creado."));
        await _cache.RemoveByPrefixAsync(VacunoCacheKeys.ListarPrefix);

        return new CreateVacunoOutput(VacunoAppMapper.ToOutput(saved));
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

    private static Vacuno CreateDomainVacuno(
        CreateVacunoCommand command,
        VacunoReferenceResolution resolution,
        VacunoTenantValidationSettings validationSettings,
        long granjaId)
    {
        try
        {
            return Vacuno.CreateNew(
                command.Codigo,
                command.Nombre,
                command.FechaNacimiento,
                command.TipoAdquisicionCode,
                command.RazaCode,
                command.ColorCode,
                command.SexoCode,
                resolution.PadreId,
                resolution.MadreId,
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
    }

    private static VacunoReferenceData ToReferenceData(CreateVacunoCommand command)
        => new(
            command.Codigo,
            command.CodigoPadre,
            command.CodigoMadre,
            command.GranjaId,
            command.Granja,
            command.CodigoDistrito);

    private static void ValidateCommand(CreateVacunoCommand command, VacunoTenantValidationSettings settings)
    {
        settings.ValidateCodigo(command.Codigo);
        settings.ValidateInput(command.Nombre, "nombre");
        settings.ValidateInput(command.TipoAdquisicionCode, "tipoAdquisicionCode");
        settings.ValidateInput(command.RazaCode, "razaCode");
        settings.ValidateInput(command.ColorCode, "colorCode");
        settings.ValidateInput(command.SexoCode, "sexoCode");
        settings.ValidateObservaciones(command.Observaciones);
    }
}
