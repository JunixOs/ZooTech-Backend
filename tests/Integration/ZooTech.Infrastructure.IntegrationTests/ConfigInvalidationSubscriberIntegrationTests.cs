using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ZooTech.Infrastructure.Parametrization;

namespace ZooTech.Infrastructure.IntegrationTests;

/// <summary>
/// Tests simplificados del suscriptor de invalidación.
/// En modo desarrollo (sin Redis) el suscriptor es un no-op que solo loguea.
/// </summary>
public sealed class ConfigInvalidationSubscriberIntegrationTests
{
    [Fact]
    public async Task Subscriber_StartsAndStops_WithoutErrors()
    {
        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(loggerMock.Object);

        await subscriber.StartAsync(CancellationToken.None);
        await subscriber.StopAsync(CancellationToken.None);
        subscriber.Dispose();

        // Si llega aquí sin excepción, el test pasa
        true.Should().BeTrue();
    }

    [Fact]
    public async Task Subscriber_Stop_WhenNotStarted_DoesNotThrow()
    {
        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(loggerMock.Object);

        var act = () => subscriber.StopAsync(CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Subscriber_Dispose_DoesNotThrow()
    {
        var loggerMock = new Mock<ILogger<ConfigInvalidationSubscriber>>();
        var subscriber = new ConfigInvalidationSubscriber(loggerMock.Object);

        await subscriber.StartAsync(CancellationToken.None);

        var act = () => { subscriber.Dispose(); return Task.CompletedTask; };
        await act.Should().NotThrowAsync();
    }
}
