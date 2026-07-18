using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Rules;

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
            ThrowValidation(
                "codigo",
                "VACUNO-CODIGO-MAX_LENGTH",
                $"El codigo del vacuno no puede superar los {CodigoMaxLength} caracteres.");

        if (CodigoUppercaseRequired && codigo.Trim() != codigo.Trim().ToUpperInvariant())
            ThrowValidation(
                "codigo",
                "VACUNO-CODIGO-UPPERCASE",
                "El codigo del vacuno debe registrarse en mayusculas.");
    }

    public void ValidateInput(string value, string fieldName)
    {
        if (value.Trim().Length > InputMaxLength)
            ThrowValidation(
                fieldName,
                "VACUNO-INPUT-MAX_LENGTH",
                $"El campo {fieldName} no puede superar los {InputMaxLength} caracteres.");
    }

    public void ValidateObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return;

        var value = observaciones.Trim();
        if (value.Length > ObservacionesMaxLength)
            ThrowValidation(
                "observaciones",
                "VACUNO-OBSERVACIONES-MAX_LENGTH",
                $"Las observaciones no pueden superar los {ObservacionesMaxLength} caracteres.");

        if (value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > ObservacionesMaxWords)
            ThrowValidation(
                "observaciones",
                "VACUNO-OBSERVACIONES-MAX_WORDS",
                $"Las observaciones no pueden superar las {ObservacionesMaxWords} palabras.");
    }

    private static void ThrowValidation(string field, string code, string message)
        => throw VacunoValidationErrorDetails.CreateException(field, code, message);
}
