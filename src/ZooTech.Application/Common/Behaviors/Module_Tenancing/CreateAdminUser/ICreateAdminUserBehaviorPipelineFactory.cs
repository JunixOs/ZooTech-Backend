using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateAdminUser
{
    public interface ICreateAdminUserBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateAdminUserCommand, CreateAdminUserOutput> Create();
    }
}