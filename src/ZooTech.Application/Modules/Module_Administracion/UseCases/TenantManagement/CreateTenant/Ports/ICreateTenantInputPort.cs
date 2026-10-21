using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant.Ports
{
    public interface ICreateTenantInputPort : IRequestHandler<CreateTenantCommand , CreateTenantOutput>
    {
    }
}