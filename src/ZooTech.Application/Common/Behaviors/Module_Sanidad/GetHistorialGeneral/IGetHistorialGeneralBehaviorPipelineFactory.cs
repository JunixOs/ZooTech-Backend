using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral
{
    public interface IGetHistorialGeneralBehaviorPipelineFactory
    {
        BehaviorPipeline<GetHistorialGeneralCommand , GetHistorialGeneralOutput> Create();
    }
}