using FluentValidation;
using ZooTech.Domain.Module_Reproduccion.Entities;

namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public sealed class ConfirmarFecundacionValidator : AbstractValidator<ConfirmarFecundacionCommand>
{
    public ConfirmarFecundacionValidator()
    {
        RuleFor(x => x.FecundacionId)
            .GreaterThan(0).WithMessage("El ID de la fecundación es requerido.");

        RuleFor(x => x.NuevoResultadoCode)
            .NotEmpty().WithMessage("El nuevo resultado es requerido.")
            .Must(x => x == Fecundacion.ResultadoExitosa || x == Fecundacion.ResultadoFallida)
            .WithMessage("El resultado solo puede cambiarse a EXITOSA o FALLIDA.");
    }
}
