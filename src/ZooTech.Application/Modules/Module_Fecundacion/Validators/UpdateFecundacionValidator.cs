using FluentValidation;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Rules;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class UpdateFecundacionValidator : AbstractValidator<UpdateFecundacionCommand>
{
    public UpdateFecundacionValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.TipoFecundacionCode).NotEmpty().MaximumLength(40);
        RuleFor(command => command.VacunoReceptorId).GreaterThan(0);
        RuleFor(command => command.FechaProcedimiento)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha del procedimiento no puede ser futura.");
        RuleFor(command => command.ResponsableNombre).NotEmpty().MaximumLength(100);
        RuleFor(command => command.ResultadoCode).NotEmpty().MaximumLength(30);
        RuleFor(command => command.EstadoFecundacionCode).NotEmpty().MaximumLength(30);
        RuleFor(command => command.ObservacionesVeterinarias).MaximumLength(250);

        RuleFor(command => command.TipoDonante)
            .NotEmpty()
            .Must(tipo => IsDonante(tipo, FecundacionRules.TipoDonanteInterno) || IsDonante(tipo, FecundacionRules.TipoDonanteExterno))
            .WithMessage("El tipo de donante debe ser INTERNO o EXTERNO.");

        When(command => IsDonante(command.TipoDonante, FecundacionRules.TipoDonanteInterno), () =>
        {
            RuleFor(command => command.VacunoDonanteId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un donante interno.");
        });

        When(command => IsDonante(command.TipoDonante, FecundacionRules.TipoDonanteExterno), () =>
        {
            RuleFor(command => command.ExternoDonanteNombre)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Debe ingresar el nombre del donante externo.");
        });

        When(command => FecundacionRules.EsInseminacionArtificial(command.TipoFecundacionCode), () =>
        {
            RuleFor(command => command.CodigoSemen)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("El código de semen es obligatorio para inseminación artificial.");
        });

        When(command => FecundacionRules.EsTransferenciaEmbriones(command.TipoFecundacionCode), () =>
        {
            RuleFor(command => command.CodigoEmbrion)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("El código de embrión es obligatorio para transferencia de embriones.");
        });
    }

    private static bool IsDonante(string? value, string expected)
        => string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
}
