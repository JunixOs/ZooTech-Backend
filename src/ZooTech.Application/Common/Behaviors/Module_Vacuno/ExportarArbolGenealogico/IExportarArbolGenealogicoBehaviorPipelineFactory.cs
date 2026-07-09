using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarArbolGenealogico
{
    public interface IExportarArbolGenealogicoBehaviorPipelineFactory
    {
        BehaviorPipeline<ExportarArbolGenealogicoCommand , ExportarArbolGenealogicoOutput> Create();
    }
}