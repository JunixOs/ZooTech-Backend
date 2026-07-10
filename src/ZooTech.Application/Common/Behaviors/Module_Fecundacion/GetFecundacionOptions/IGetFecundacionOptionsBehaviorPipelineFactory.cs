using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

namespace ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionOptions
{
    public interface IGetFecundacionOptionsBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand , GetFecundacionOptionsOutput> Create();
    }
}