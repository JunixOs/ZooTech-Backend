using FluentValidation;
using ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;

namespace ZooTech.Application.Modules.Module_Celo.Validators;

internal sealed class EditarCeloValidator : AbstractValidator<EditarCeloCommand>
{
    public EditarCeloValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID del celo debe ser mayor que cero.");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 500 caracteres.");
    }
}
