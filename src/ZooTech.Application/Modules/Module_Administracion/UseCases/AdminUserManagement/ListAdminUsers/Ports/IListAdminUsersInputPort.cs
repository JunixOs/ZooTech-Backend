using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.ListAdminUsers;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.ListAdminUsers.Ports
{
    public interface IListAdminUsersInputPort
        : IRequestHandler<ListAdminUsersQuery , List<ListAdminUsersOutput>>
    {
    }
}