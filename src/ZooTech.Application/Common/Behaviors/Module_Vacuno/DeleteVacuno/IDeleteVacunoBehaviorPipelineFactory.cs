using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.DeleteVacuno
{
    public interface IDeleteVacunoBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteVacunoCommand , EmptyOutput> Create();
    }
}