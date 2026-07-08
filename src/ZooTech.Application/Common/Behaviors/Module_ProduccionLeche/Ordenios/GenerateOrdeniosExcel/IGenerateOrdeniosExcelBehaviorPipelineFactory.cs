using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosExcel
{
    public interface IGenerateOrdeniosExcelBehaviorPipelineFactory
    {
        BehaviorPipeline<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput> Create();
    }
}