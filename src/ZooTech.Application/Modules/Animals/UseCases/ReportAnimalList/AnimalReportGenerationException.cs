namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class AnimalReportGenerationException : Exception
{
    public AnimalReportGenerationException(Exception innerException)
        : base("No se pudo generar el reporte de vacunos.", innerException)
    {
    }
}
