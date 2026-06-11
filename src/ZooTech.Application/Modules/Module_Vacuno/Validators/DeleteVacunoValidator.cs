using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class DeleteVacunoValidator : AbstractValidator<DeleteVacunoCommand>
{
    public DeleteVacunoValidator()
    {
        RuleFor(x => x.MotivoEliminacion)
            .NotEmpty().WithMessage("El motivo de eliminación es obligatorio.")
            .MaximumLength(200).WithMessage("El motivo de eliminación no puede superar los 200 caracteres.");
    }
}
