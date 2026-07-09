using ZooTech.Application.Common.Gateway.Repositories.Parametrization;

namespace ZooTech.Infrastructure.Parametrization.Features;

public sealed class FeatureRepository : IFeatureRepository
{
    public Task<HashSet<string>> GetEnabledFeatureCodesAsync(int tenantId)
    {
        throw new NotImplementedException("FeatureRepository must be configured with a database provider.");
    }
}
