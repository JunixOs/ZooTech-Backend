using FluentValidation;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators;

internal sealed class CreateOrdenioValidator : AbstractValidator<CreateOrdenioCommand>
{
    public CreateOrdenioValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del ordeño es obligatorio.")
            .MaximumLength(50).WithMessage("El código no puede superar los 50 caracteres.");

        RuleFor(x => x.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora del ordeño es obligatoria.");

        RuleFor(x => x.VacunoId)
            .GreaterThan(0).WithMessage("El ID del vacuno debe ser mayor que cero.");

        RuleFor(x => x.EncargadoUsuarioId)
            .GreaterThan(0).WithMessage("El ID del encargado debe ser mayor que cero.");

        RuleFor(x => x.Litros)
            .GreaterThan(0).WithMessage("Los litros deben ser mayor que cero.");

        RuleFor(x => x.EstadoOrdenioCode)
            .NotEmpty().WithMessage("El estado del ordeño es obligatorio.");

        RuleFor(x => x.Observaciones)
            .MaximumLength(150).When(x => x.Observaciones is not null)
            .WithMessage("Las observaciones no pueden superar los 150 caracteres.");
    }
}
