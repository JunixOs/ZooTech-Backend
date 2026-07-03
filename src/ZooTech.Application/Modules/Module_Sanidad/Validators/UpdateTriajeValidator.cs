using FluentValidation;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class UpdateTriajeValidator : AbstractValidator<UpdateTriajeCommand>
{
    public UpdateTriajeValidator()
    {
        RuleFor(x => x.TipoPesoCode)
            .NotEmpty().WithMessage("El tipo de peso es obligatorio.");

        RuleFor(x => x.PesoKg)
            .GreaterThan(0).WithMessage("El peso debe ser mayor que cero.");
        RuleFor(x => x.Observaciones)
            .MaximumLength(150).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");
    }
}
