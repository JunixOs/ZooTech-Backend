using FluentValidation;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal static class VacunoValidationRules
{
    public static IRuleBuilderOptions<T, string> VacunoNombreRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("El nombre del vacuno es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

    public static IRuleBuilderOptions<T, DateOnly> VacunoFechaNacimientoRules<T>(this IRuleBuilder<T, DateOnly> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.");

    public static IRuleBuilderOptions<T, string> VacunoTipoAdquisicionCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("El tipo de adquisición es obligatorio.")
            .MaximumLength(30).WithMessage("El código de tipo de adquisición no puede superar los 30 caracteres.");

    public static IRuleBuilderOptions<T, string> VacunoRazaCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("La raza es obligatoria.")
            .MaximumLength(30).WithMessage("El código de raza no puede superar los 30 caracteres.");

    public static IRuleBuilderOptions<T, string> VacunoColorCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("El color es obligatorio.")
            .MaximumLength(30).WithMessage("El código de color no puede superar los 30 caracteres.");

    public static IRuleBuilderOptions<T, string> VacunoSexoCodeRules<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("El sexo es obligatorio.")
            .MaximumLength(10).WithMessage("El código de sexo no puede superar los 10 caracteres.");

    public static IRuleBuilderOptions<T, long> VacunoGranjaIdRules<T>(this IRuleBuilder<T, long> ruleBuilder) =>
        ruleBuilder.GreaterThan(0).WithMessage("El ID de la granja debe ser mayor que cero.");

    public static IRuleBuilderOptions<T, string?> VacunoObservacionesRules<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<T, bool> condition) =>
        ruleBuilder
            .MaximumLength(150).When(condition)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");
}
