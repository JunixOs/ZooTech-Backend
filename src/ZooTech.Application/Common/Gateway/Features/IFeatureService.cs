namespace ZooTech.Application.Common.Gateway.Features;

public interface IFeatureService
{
    Task<bool> IsEnabledAsync(string feature);
}
