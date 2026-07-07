using FluentValidation;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public sealed class GetArbolGenealogicoCommandValidator : AbstractValidator<GetArbolGenealogicoCommand>
{
    public GetArbolGenealogicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID del vacuno debe ser mayor a 0.");

        RuleFor(x => x.Niveles)
            .InclusiveBetween(1, 4)
            .WithMessage("Los niveles de búsqueda genealógica deben estar entre 1 y 4 (como máximo).");
    }
}
