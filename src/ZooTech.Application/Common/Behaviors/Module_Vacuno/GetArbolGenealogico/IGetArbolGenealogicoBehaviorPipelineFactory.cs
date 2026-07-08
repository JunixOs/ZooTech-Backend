using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetArbolGenealogico
{
    public interface IGetArbolGenealogicoBehaviorPipelineFactory
    {
        BehaviorPipeline<GetArbolGenealogicoCommand , GetArbolGenealogicoOutput> Create();
    }
}