using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public interface IVacunoResponseEnricher
{
    Task<VacunoResponse> EnrichAsync(
        VacunoOutput dto,
        CancellationToken cancellationToken = default);
}
