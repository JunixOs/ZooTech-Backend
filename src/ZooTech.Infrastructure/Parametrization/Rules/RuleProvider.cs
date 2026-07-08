using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization.Rules;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.Infrastructure.Parametrization.Rules;

public sealed class RuleProvider : IRuleProvider
{
    private readonly IAppCacheService _cache;
    private readonly IRuleRepository _repository;

    public RuleProvider(IAppCacheService cache, IRuleRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<bool> IsEnabledAsync(int tenantId, RuleCode rule)
    {
        var enabled = await GetEnabledRuleCodesAsync(tenantId);
        return enabled.Contains(rule.Value);
    }

    private async Task<HashSet<string>> GetEnabledRuleCodesAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        var (found, cached) = await _cache.TryGetAsync<HashSet<string>>(key);
        if (found)
            return cached!;

        var codes = await _repository.GetEnabledRuleCodesAsync(tenantId);
        await _cache.GetOrCreateAsync(key, () => Task.FromResult(codes));

        return codes;
    }

    private static string BuildKey(int tenantId) => $"rules:tenant:{tenantId}";
}
