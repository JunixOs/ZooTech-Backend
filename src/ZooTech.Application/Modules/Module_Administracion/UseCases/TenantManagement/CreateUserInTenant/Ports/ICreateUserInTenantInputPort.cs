using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateUserInTenant;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateUserInTenant.Ports
{
    public interface ICreateUserInTenantInputPort : IRequestHandler<CreateUserInTenantCommand , CreateUserInTenantOutput>
    {
    }
}