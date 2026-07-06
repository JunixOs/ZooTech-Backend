using ZooTech.Application.Common.Gateway.Repositories.Parametrization;

namespace ZooTech.Infrastructure.Parametrization.Rules;

public sealed class RuleRepository : IRuleRepository
{
    public Task<HashSet<string>> GetEnabledRuleCodesAsync(int tenantId)
    {
        throw new NotImplementedException("RuleRepository must be configured with a database provider.");
    }
}
