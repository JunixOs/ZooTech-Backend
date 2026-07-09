using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.ListarVacunos
{
    public interface IListarVacunosBehaviorPipelineFactory
    {
        BehaviorPipeline<ListarVacunosCommand , ListarVacunosOutput> Create();
    }
}