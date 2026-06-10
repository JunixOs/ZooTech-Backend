using FluentValidation;
using ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;

namespace ZooTech.Application.Modules.Module_Celo.Validators;

internal sealed class EliminarCeloValidator : AbstractValidator<EliminarCeloCommand>
{
    public EliminarCeloValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID del celo debe ser mayor que cero.");

        RuleFor(x => x.MotivoEliminacion)
            .NotEmpty().WithMessage("El motivo de eliminación es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres.");
    }
}
