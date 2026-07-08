using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.GetFecundacionEstado
{
    public interface IGetFecundacionEstadoBehaviorPipelineFactory
    {
        BehaviorPipeline<GetFecundacionEstadoCommand , GetFecundacionEstadoOutput> Create();
    }
}