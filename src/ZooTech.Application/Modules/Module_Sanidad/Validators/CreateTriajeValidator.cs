using FluentValidation;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class CreateTriajeValidator : AbstractValidator<CreateTriajeCommand>
{
    public CreateTriajeValidator()
    {
        RuleFor(x => x.VacunoId)
            .GreaterThan(0).WithMessage("El vacuno debe ser obligatorio.");

        RuleFor(x => x.TipoPesoCode)
            .NotEmpty().WithMessage("El tipo de peso es obligatorio.");

        RuleFor(x => x.PesoKg)
            .GreaterThan(0).WithMessage("El peso debe ser mayor que cero.");
        RuleFor(x => x.Observaciones)
            .MaximumLength(150).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");

        RuleFor(x => x.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora es obligatoria.")
            .Must(fechaHora => fechaHora <= DateTime.UtcNow)
            .WithMessage("La fecha y hora del triaje no puede ser futura.");
    }
}
