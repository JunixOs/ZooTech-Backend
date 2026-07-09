using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.ListarFecundacion
{
    public interface IListarFecundacionBehaviorPipelineFactory
    {
        BehaviorPipeline<ListarFecundacionCommand , ListarFecundacionOutput> Create();
    }
}