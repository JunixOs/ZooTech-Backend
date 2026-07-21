using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports
{
    public interface IListAdminUsersInputPort
        : IRequestHandler<ListAdminUsersQuery , List<ListAdminUsersOutput>>
    {
    }
}