using FluentValidation;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators;

internal sealed class UpdateOrdenioValidator : AbstractValidator<UpdateOrdenioCommand>
{
    public UpdateOrdenioValidator()
    {
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
