using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.CreateFecundacion
{
    public interface ICreateFecundacionBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateFecundacionCommand , CreateFecundacionOutput> Create();
    }
}