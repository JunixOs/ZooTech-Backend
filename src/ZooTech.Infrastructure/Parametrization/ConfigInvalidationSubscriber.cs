using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZooTech.Infrastructure.Parametrization;

/// <summary>
/// Versión no-operacional del suscriptor de invalidación Redis/Garnet.
/// En desarrollo local no se requiere servidor de caché externo.
/// </summary>
public sealed class ConfigInvalidationSubscriber : IHostedService, IDisposable
{
    private readonly ILogger<ConfigInvalidationSubscriber> _logger;

    public ConfigInvalidationSubscriber(ILogger<ConfigInvalidationSubscriber> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConfigInvalidationSubscriber: modo desarrollo (sin Redis). Cache multi-instancia desactivado.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose() { }
}
