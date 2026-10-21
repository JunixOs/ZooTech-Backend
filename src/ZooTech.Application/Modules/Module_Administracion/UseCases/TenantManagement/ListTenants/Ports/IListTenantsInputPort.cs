using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants.Ports
{
    public interface IListTenantsInputPort : IRequestHandler<ListTenantsQuery , List<ListTenantsOutput>>
    {
    }
}