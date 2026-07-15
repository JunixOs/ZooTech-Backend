using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Rules;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Common;

internal sealed record VacunoTenantValidationSettings(
    int CodigoMaxLength,
    int InputMaxLength,
    int ObservacionesMaxLength,
    int ObservacionesMaxWords,
    bool CodigoUppercaseRequired)
{
    public VacunoValidationLimits ToDomainLimits()
        => new(
            CodigoMaxLength,
            InputMaxLength,
            ObservacionesMaxLength,
            ObservacionesMaxWords);

    public static async Task<VacunoTenantValidationSettings> LoadAsync(ITenantConfigurationProvider provider)
        => new(
            await provider.GetSettingAsync(Settings.Vacunos.VacunosCodigoMaxLength),
            await provider.GetSettingAsync(Settings.Vacunos.VacunosInputMaxLength),
            await provider.GetSettingAsync(Settings.Vacunos.VacunosObservacionesMaxLength),
            await provider.GetSettingAsync(Settings.Vacunos.VacunosObservacionesMaxWords),
            await provider.GetSettingAsync(Settings.Vacunos.VacunosCodigoUppercaseRequired));

    public void ValidateCodigo(string codigo)
    {
        if (codigo.Trim().Length > CodigoMaxLength)
            ThrowValidation($"El codigo del vacuno no puede superar los {CodigoMaxLength} caracteres.");

        if (CodigoUppercaseRequired && codigo.Trim() != codigo.Trim().ToUpperInvariant())
            ThrowValidation("El codigo del vacuno debe registrarse en mayusculas.");
    }

    public void ValidateInput(string value, string fieldName)
    {
        if (value.Trim().Length > InputMaxLength)
            ThrowValidation($"El campo {fieldName} no puede superar los {InputMaxLength} caracteres.");
    }

    public void ValidateObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return;

        var value = observaciones.Trim();
        if (value.Length > ObservacionesMaxLength)
            ThrowValidation($"Las observaciones no pueden superar los {ObservacionesMaxLength} caracteres.");

        if (value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > ObservacionesMaxWords)
            ThrowValidation($"Las observaciones no pueden superar las {ObservacionesMaxWords} palabras.");
    }

    private static void ThrowValidation(string message)
        => throw new VacunoException(ErrorType.Validation, "BAD_REQUEST", message: message);
}
