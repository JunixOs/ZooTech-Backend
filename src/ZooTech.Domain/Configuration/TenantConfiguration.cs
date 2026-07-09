namespace ZooTech.Domain.Configuration;

public sealed record TenantConfiguration
{
    public int TenantId { get; init; }
    public Dictionary<string, string> Settings { get; init; } = new();
    public HashSet<string> EnabledFeatures { get; init; } = new();
    public HashSet<string> EnabledRules { get; init; } = new();
    public DateTime LoadedAt { get; init; }
}
