using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;

namespace ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin
{
    public interface IRegularLoginBehaviorPipelineFactory
    {
        BehaviorPipeline<RegularLoginCommand, string> Create();
    }
}