namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public interface IAnimalReportRepository
{
    Task<IReadOnlyCollection<ReportAnimalListItem>> GetAnimalListAsync(
        ReportAnimalListFilter filter,
        CancellationToken cancellationToken = default);
}
