using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateUserInTenant
{
    public interface ICreateUserInTenantBehaviorPipelineFactory
    {
        BehaviorPipeline<CreateUserInTenantCommand, CreateUserInTenantOutput> Create();
    }
}