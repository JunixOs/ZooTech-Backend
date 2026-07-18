using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos
{
    public interface IGetAllTipoPesosBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand , GetAllTipoPesosOutput> Create();
    }
}