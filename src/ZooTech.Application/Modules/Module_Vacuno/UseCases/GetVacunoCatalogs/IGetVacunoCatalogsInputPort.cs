using ZooTech.Domain.Module_Vacuno.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;

public interface IGetVacunoCatalogsInputPort
{
    Task<VacunoCatalogs> HandleAsync(CancellationToken cancellationToken = default);
}
