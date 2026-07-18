using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.ListAdminUsers
{
    public interface IListAdminUsersBehaviorPipelineFactory
    {
        BehaviorPipeline<EmptyCommand, List<ListAdminUsersOutput>> Create();
    }
}