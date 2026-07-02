using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing
{
    public interface ICreateTenantPipelineFactory
    {
        BehaviorPipeline<CreateTenantCommand, CreateTenantOutput> Create();
    }
}