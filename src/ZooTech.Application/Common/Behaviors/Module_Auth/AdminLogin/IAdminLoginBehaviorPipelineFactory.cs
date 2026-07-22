using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;

namespace ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin
{
    public interface IAdminLoginBehaviorPipelineFactory
    {
        BehaviorPipeline<AdminLoginCommand, string> Create();
    }
}