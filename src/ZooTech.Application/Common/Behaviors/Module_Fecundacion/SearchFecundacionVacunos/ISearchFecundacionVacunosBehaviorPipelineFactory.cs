using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.SearchFecundacionVacunos
{
    public interface ISearchFecundacionVacunosBehaviorPipelineFactory
    {
        BehaviorPipeline<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>> Create();
    }
}