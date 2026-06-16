using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Features;

public interface IFeatureProvider
{
    Task<bool> IsEnabledAsync(int tenantId, FeatureCode feature);
}
