using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class CreateVacunoValidator : AbstractValidator<CreateVacunoCommand>
{
    public CreateVacunoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del vacuno es obligatorio.")
            .MaximumLength(15).WithMessage("El código no puede superar los 15 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del vacuno es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.FechaNacimiento)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.");

        RuleFor(x => x.TipoAdquisicionCode)
            .NotEmpty().WithMessage("El tipo de adquisición es obligatorio.")
            .MaximumLength(30).WithMessage("El código de tipo de adquisición no puede superar los 30 caracteres.");

        RuleFor(x => x.RazaCode)
            .NotEmpty().WithMessage("La raza es obligatoria.")
            .MaximumLength(30).WithMessage("El código de raza no puede superar los 30 caracteres.");

        RuleFor(x => x.ColorCode)
            .NotEmpty().WithMessage("El color es obligatorio.")
            .MaximumLength(30).WithMessage("El código de color no puede superar los 30 caracteres.");

        RuleFor(x => x.SexoCode)
            .NotEmpty().WithMessage("El sexo es obligatorio.")
            .MaximumLength(10).WithMessage("El código de sexo no puede superar los 10 caracteres.");

        RuleFor(x => x.GranjaId)
            .GreaterThan(0).WithMessage("El ID de la granja debe ser mayor que cero.");

        RuleFor(x => x.Observaciones)
            .MaximumLength(150).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");
    }
}
