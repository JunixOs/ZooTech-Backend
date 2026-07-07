using FluentValidation;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed class ListarFecundacionCommandValidator : AbstractValidator<ListarFecundacionCommand>
{
    public ListarFecundacionCommandValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La página solicitada debe ser igual o mayor a 1.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100)
            .WithMessage("El límite de registros por página debe estar entre 1 y 100.");
    }
}
