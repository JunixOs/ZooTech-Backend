using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.CreateAdminUser;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.CreateAdminUser.Ports
{
    public interface ICreateAdminUserInputPort : IRequestHandler<CreateAdminUserCommand , CreateAdminUserOutput>
    {
    }
}