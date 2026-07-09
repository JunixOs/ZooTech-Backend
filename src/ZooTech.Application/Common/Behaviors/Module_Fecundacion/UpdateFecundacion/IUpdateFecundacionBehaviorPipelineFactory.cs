using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.UpdateFecundacion
{
    public interface IUpdateFecundacionBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateFecundacionCommand , UpdateFecundacionOutput> Create();
    }
}