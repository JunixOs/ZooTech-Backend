using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants.Ports;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants
{
    public class ListTenantsInteractor : IListTenantsInputPort
    {
        public readonly ITenantRepository _tenantRepository;

        public ListTenantsInteractor(
            ITenantRepository tenantRepository
        )
        {
            _tenantRepository = tenantRepository;
        }

        public Task<List<ListTenantsOutput>> HandleAsync(ListTenantsQuery request, CancellationToken cancellationToken)
        {
            return _tenantRepository.ListAllTenants(cancellationToken);
        }
    }
}