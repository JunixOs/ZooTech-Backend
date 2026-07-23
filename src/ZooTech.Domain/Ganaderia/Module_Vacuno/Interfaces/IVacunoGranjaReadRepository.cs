using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoGranjaReadRepository
{
    Task<IReadOnlyList<VacunoGranjaListItem>> ListarActivasAsync(
        CancellationToken cancellationToken = default);
}
