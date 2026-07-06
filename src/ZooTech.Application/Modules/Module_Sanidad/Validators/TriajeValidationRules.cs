using FluentValidation;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal static class TriajeValidationRules
{
    public static IRuleBuilderOptions<T, long> TriajeVacunoIdRules<T>(this IRuleBuilder<T, long> ruleBuilder) =>
        ruleBuilder.GreaterThan(0).WithMessage("El ID del vacuno debe ser mayor que cero.");

    public static IRuleBuilderOptions<T, string> TriajeTipoPesoCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("El tipo de peso es obligatorio.");

    public static IRuleBuilderOptions<T, decimal> TriajePesoKgRules<T>(this IRuleBuilder<T, decimal> ruleBuilder) =>
        ruleBuilder.GreaterThan(0).WithMessage("El peso debe ser mayor que cero.");

    public static IRuleBuilderOptions<T, string> TriajeEstadoRegistroCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("El estado de registro es obligatorio.");

    public static IRuleBuilderOptions<T, string?> TriajeObservacionesRules<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<T, bool> condition) =>
        ruleBuilder
            .MaximumLength(500).When(condition)
            .WithMessage("Las observaciones no pueden superar los 500 caracteres.");
}
