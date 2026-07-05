using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.DeleteAdminUser
{
    public interface IDeleteAdminUserBehaviorPipelineFactory
    {
        BehaviorPipeline<DeleteAdminUserCommand, EmptyOutput> Create();
    }
}