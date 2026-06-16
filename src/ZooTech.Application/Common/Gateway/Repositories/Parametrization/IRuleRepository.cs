namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

public interface IRuleRepository
{
    Task<HashSet<string>> GetEnabledRuleCodesAsync(int tenantId);
}
