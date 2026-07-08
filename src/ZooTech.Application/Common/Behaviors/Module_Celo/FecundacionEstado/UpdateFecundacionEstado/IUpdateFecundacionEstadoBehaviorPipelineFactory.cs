using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.UpdateFecundacionEstado
{
    public interface IUpdateFecundacionEstadoBehaviorPipelineFactory
    {
        BehaviorPipeline<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput> Create();
    }
}