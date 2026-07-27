using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;

public interface IGetVacunoCatalogsInputPort
    : IRequestHandler<EmptyCommandQuery , VacunoCatalogs>
{
}
