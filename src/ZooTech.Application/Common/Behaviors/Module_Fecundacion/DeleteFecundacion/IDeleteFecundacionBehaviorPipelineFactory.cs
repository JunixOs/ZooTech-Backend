using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.DeleteFecundacion
{
    public interface IDeleteFecundacionBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteFecundacionCommand , EmptyOutput> Create();
    }
}