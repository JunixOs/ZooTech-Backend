using FluentValidation;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators;

internal sealed class DeleteOrdenioValidator : AbstractValidator<DeleteOrdenioCommand>
{
    public DeleteOrdenioValidator()
    {
        RuleFor(x => x.MotivoEliminacion)
            .NotEmpty().WithMessage("El motivo de eliminación es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres.");
    }
}
