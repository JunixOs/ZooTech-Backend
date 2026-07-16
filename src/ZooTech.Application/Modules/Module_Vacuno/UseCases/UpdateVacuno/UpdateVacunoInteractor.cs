using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed class UpdateVacunoInteractor : IUpdateVacunoInputPort
{
    private readonly IGanaderiaUnitOfWork _unitOfWork;
    private readonly IAppCacheService _cache;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public UpdateVacunoInteractor(
        IGanaderiaUnitOfWork unitOfWork,
        IAppCacheService cache,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _tenantConfigurationProvider = tenantConfigurationProvider;
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

        try
        {
            existing.Update(
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

        var updated = await _unitOfWork.ExecuteInTransactionAsync(
            ct => repository.UpdateAsync(existing, command.PrecioCompra, command.AptoPara, ct),
            cancellationToken);
        await _cache.RemoveByPrefixAsync(VacunoCacheKeys.ListarPrefix);

        return new UpdateVacunoOutput(VacunoAppMapper.ToOutput(updated));
    }

    private static void ValidateCommand(UpdateVacunoCommand command, VacunoTenantValidationSettings settings)
    {
        settings.ValidateInput(command.Nombre, nameof(command.Nombre));
        settings.ValidateInput(command.TipoAdquisicionCode, nameof(command.TipoAdquisicionCode));
        settings.ValidateInput(command.RazaCode, nameof(command.RazaCode));
        settings.ValidateInput(command.ColorCode, nameof(command.ColorCode));
        settings.ValidateInput(command.SexoCode, nameof(command.SexoCode));
        settings.ValidateObservaciones(command.Observaciones);
    }
}
