using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants.Ports;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants
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