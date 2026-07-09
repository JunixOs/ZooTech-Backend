using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Vacuno.Models;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoCatalogs
{
    public interface IGetVacunoCatalogsBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand , VacunoCatalogs> Create();
    }
}