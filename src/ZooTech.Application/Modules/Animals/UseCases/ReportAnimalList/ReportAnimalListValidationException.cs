namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class ReportAnimalListValidationException : Exception
{
    public ReportAnimalListValidationException(IReadOnlyDictionary<string, string> errors)
        : base("Los filtros del reporte de vacunos no son validos.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
