using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.Common;

public interface IFecundacionObservationPolicy
{
    Task ValidateAsync(
        string? observaciones,
        CancellationToken cancellationToken = default);
}

public sealed class FecundacionObservationPolicy : IFecundacionObservationPolicy
{
    private const int DatabaseMaxLength = 250;
    private readonly ITenantConfigurationProvider _configurationProvider;

    public FecundacionObservationPolicy(
        ITenantConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;
    }

    public async Task ValidateAsync(
        string? observaciones,
        CancellationToken cancellationToken = default)
    {
        var maxLength = await _configurationProvider.GetSettingAsync(
            Settings.Vacunos.VacunosFecundacionObservacionesMaxLength);

        if (maxLength is < 1 or > DatabaseMaxLength)
        {
            throw CreateValidationException(
                "configuracion",
                "FECUNDACION-CONFIG-OBSERVACIONES_MAX_LENGTH-INVALID",
                $"El límite de observaciones de fecundación debe estar entre 1 y {DatabaseMaxLength}.");
        }

        if (!string.IsNullOrWhiteSpace(observaciones) &&
            observaciones.Length > maxLength)
        {
            throw CreateValidationException(
                "observacionesVeterinarias",
                "FECUNDACION-OBSERVACIONES_VETERINARIAS-MAX_LENGTH",
                $"Las observaciones veterinarias no pueden superar los {maxLength} caracteres.");
        }
    }

    private static ValidationException CreateValidationException(
        string field,
        string code,
        string message)
        => new(
            [code],
            ScopeName.Application,
            ModuleName.Fecundacion,
            [new FieldValidationError(field, code, message)],
            message);
}
