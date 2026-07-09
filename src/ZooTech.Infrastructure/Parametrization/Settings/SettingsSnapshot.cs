namespace ZooTech.Infrastructure.Parametrization.Settings;

public sealed class SettingsSnapshot
{
    public Dictionary<string, string> Values { get; init; } = new();
}
