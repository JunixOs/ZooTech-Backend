using FluentValidation;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal static class TriajeValidationRules
{
    public static IRuleBuilderOptions<T, long> TriajeVacunoIdRules<T>(this IRuleBuilder<T, long> ruleBuilder) =>
        ruleBuilder.GreaterThan(0).WithMessage("El vacuno debe ser obligatorio.");

    public static IRuleBuilderOptions<T, string> TriajeTipoPesoCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("El tipo de peso es obligatorio.");

    public static IRuleBuilderOptions<T, decimal> TriajePesoKgRules<T>(this IRuleBuilder<T, decimal> ruleBuilder) =>
        ruleBuilder.GreaterThan(0).WithMessage("El peso debe ser mayor que cero.");

    public static IRuleBuilderOptions<T, string?> TriajeObservacionesRules<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<T, bool> condition) =>
        ruleBuilder
            .MaximumLength(150).When(condition)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");

    public static IRuleBuilderOptions<T, DateTime> TriajeFechaHoraRules<T>(this IRuleBuilder<T, DateTime> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("La fecha y hora es obligatoria.")
            .Must(fechaHora => fechaHora <= DateTime.UtcNow)
            .WithMessage("La fecha y hora del triaje no puede ser futura.");
}
