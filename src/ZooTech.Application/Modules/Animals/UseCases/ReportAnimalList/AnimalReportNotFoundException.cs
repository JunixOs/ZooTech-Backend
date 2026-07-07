namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class AnimalReportNotFoundException : Exception
{
    public AnimalReportNotFoundException()
        : base("No se encontraron vacunos para los filtros enviados.")
    {
    }
}
