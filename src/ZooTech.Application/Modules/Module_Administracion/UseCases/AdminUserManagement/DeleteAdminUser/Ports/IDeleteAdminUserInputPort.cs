using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.DeleteAdminUser;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.DeleteAdminUser.Ports
{
    public interface IDeleteAdminUserInputPort : IRequestHandler<DeleteAdminUserCommand , EmptyOutput>
    {
    }
}