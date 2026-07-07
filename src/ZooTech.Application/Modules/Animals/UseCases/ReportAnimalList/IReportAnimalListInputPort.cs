namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public interface IReportAnimalListInputPort
{
    Task Handle(
        ReportAnimalListCommand command,
        IReportAnimalListOutputPort outputPort,
        CancellationToken cancellationToken = default);
}
