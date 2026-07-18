using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.DeleteOrdenio
{
    public interface IDeleteOrdenioBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteOrdenioCommand , EmptyOutput> Create();
    }
}