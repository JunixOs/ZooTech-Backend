using FluentValidation;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class DeleteTriajeValidator : AbstractValidator<DeleteTriajeCommand>
{
    public DeleteTriajeValidator()
    {
        RuleFor(x => x.MotivoEliminacion)
            .NotEmpty().WithMessage("El motivo de eliminación es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres.");
    }
}
