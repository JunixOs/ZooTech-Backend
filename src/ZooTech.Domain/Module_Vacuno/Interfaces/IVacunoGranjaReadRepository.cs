using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoGranjaReadRepository
{
    Task<IReadOnlyList<VacunoGranjaListItem>> ListarActivasAsync(
        CancellationToken cancellationToken = default);
}
