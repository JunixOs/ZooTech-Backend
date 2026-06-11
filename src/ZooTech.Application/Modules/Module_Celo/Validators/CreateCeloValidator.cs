using FluentValidation;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

namespace ZooTech.Application.Modules.Module_Celo.Validators;

internal sealed class CreateCeloValidator : AbstractValidator<CreateCeloCommand>
{
    public CreateCeloValidator()
    {
        RuleFor(x => x.VacunoId)
            .GreaterThan(0).WithMessage("El ID del vacuno debe ser mayor que cero.");

        RuleFor(x => x.EncargadoUsuarioId)
            .GreaterThan(0).WithMessage("El ID del encargado debe ser mayor que cero.");

        RuleFor(x => x.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora del celo es obligatoria.");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 500 caracteres.");

        RuleFor(x => x.CaracteristicaCodes)
            .NotEmpty().WithMessage("Debe indicar al menos una característica.");
    }
}
