using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

namespace ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosPdf
{
    public interface IGenerateOrdeniosPdfBehaviorPipelineFactory
    {
        BehaviorPipeline<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput> Create();
    }
}