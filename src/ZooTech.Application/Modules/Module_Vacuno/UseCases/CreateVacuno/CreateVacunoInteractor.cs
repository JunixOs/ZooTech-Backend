using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed class CreateVacunoInteractor : ICreateVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IAppCacheService _cache;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public CreateVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IAppCacheService cache,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken)
    {
        var validationSettings = await VacunoTenantValidationSettings.LoadAsync(_tenantConfigurationProvider);
        ValidateCommand(command, validationSettings);
        var repository = _unitOfWork.Vacunos;

        if (await repository.ExistsCodigoAsync(command.Codigo, cancellationToken))
            throw new VacunoAlreadyExistsException("Ya existe un vacuno con ese codigo o datos repetidos.");

        Vacuno vacuno;
        try
        {
            vacuno = Vacuno.CreateNew(
                command.Codigo,
                command.Nombre,
                command.FechaNacimiento,
                command.TipoAdquisicionCode,
                command.RazaCode,
                command.ColorCode,
                command.SexoCode,
                command.PadreId,
                command.MadreId,
                command.GranjaId,
                command.Observaciones,
                validationSettings.ToDomainLimits(),
                null,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            throw new VacunoException(
                ErrorType.Validation,
                "BAD_REQUEST",
                message: ex.Message);
        }

        var saved = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.AddAsync(vacuno, command.PrecioCompra, command.AptoPara, ct),
            cancellationToken,
            async (_, ct) => await repository.GetByCodigoAsync(command.Codigo, ct)
                ?? throw new InvalidOperationException("No se pudo recuperar el vacuno creado."));
        await _cache.RemoveByPrefixAsync(VacunoCacheKeys.ListarPrefix);

        return new CreateVacunoOutput(VacunoAppMapper.ToOutput(saved));
    }

    private static void ValidateCommand(CreateVacunoCommand command, VacunoTenantValidationSettings settings)
    {
        settings.ValidateCodigo(command.Codigo);
        settings.ValidateInput(command.Nombre, nameof(command.Nombre));
        settings.ValidateInput(command.TipoAdquisicionCode, nameof(command.TipoAdquisicionCode));
        settings.ValidateInput(command.RazaCode, nameof(command.RazaCode));
        settings.ValidateInput(command.ColorCode, nameof(command.ColorCode));
        settings.ValidateInput(command.SexoCode, nameof(command.SexoCode));
        settings.ValidateObservaciones(command.Observaciones);
    }
}
