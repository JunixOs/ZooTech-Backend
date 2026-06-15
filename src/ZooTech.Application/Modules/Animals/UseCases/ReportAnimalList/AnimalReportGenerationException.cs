namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class AnimalReportGenerationException : Exception
{
    public AnimalReportGenerationException(Exception innerException)
        : base("No se pudo generar el reporte de vacunos.", innerException)
    {
        OriginalExceptionMessage = innerException.Message;
        OriginalInnerExceptionMessage = innerException.InnerException?.Message;
        OriginalExceptionTypeName = innerException.GetType().Name;
    }

    public string OriginalExceptionMessage { get; }

    public string? OriginalInnerExceptionMessage { get; }

    public string OriginalExceptionTypeName { get; }
}
