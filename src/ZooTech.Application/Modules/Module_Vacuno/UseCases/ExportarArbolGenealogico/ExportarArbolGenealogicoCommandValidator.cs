using FluentValidation;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed class ExportarArbolGenealogicoCommandValidator : AbstractValidator<ExportarArbolGenealogicoCommand>
{
    public ExportarArbolGenealogicoCommandValidator()
    {
        RuleFor(x => x.Niveles)
            .InclusiveBetween(1, 4)
            .WithMessage("Los niveles de búsqueda genealógica deben estar entre 1 y 4 (como máximo).");
    }
}
