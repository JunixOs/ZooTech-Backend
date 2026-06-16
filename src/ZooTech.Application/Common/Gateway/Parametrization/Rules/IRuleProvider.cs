using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Rules;

public interface IRuleProvider
{
    Task<bool> IsEnabledAsync(int tenantId, RuleCode rule);
}
