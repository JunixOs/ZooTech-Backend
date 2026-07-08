using ZooTech.Application.Common.Gateway.Features;

namespace ZooTech.Infrastructure.Features;

/// <summary>
/// Implementación de desarrollo de IFeatureService.
/// Habilita todos los features para cualquier tenant, permitiendo
/// el desarrollo y testing sin bloqueos por Feature Flags.
/// En producción, reemplazar por una implementación que consulte
/// la base de datos o un servicio de configuración por tenant.
/// </summary>
public class DevFeatureService : IFeatureService
{
    public Task<bool> IsEnabledAsync(string feature)
    {
        // En desarrollo, todos los features están habilitados
        return Task.FromResult(true);
    }
}
