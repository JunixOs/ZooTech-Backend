namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

public interface IFeatureRepository
{
    Task<HashSet<string>> GetEnabledFeatureCodesAsync(int tenantId);
}
