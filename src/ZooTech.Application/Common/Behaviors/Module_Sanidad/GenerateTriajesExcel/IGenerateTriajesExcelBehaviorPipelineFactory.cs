using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel
{
    public interface IGenerateTriajesExcelBehaviorPipelineFactory
    {
        BehaviorPipeline<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput> Create();
    }
}