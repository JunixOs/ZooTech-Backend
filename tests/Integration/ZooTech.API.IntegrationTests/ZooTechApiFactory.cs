using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Infrastructure.Caching;

namespace ZooTech.API.IntegrationTests;

public sealed class ZooTechApiFactory : WebApplicationFactory<Program>
{
    public const string DefaultTenantHost = "zootecniaunas.zentrycorp.local";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<GarnetCacheConnection>();
            services.RemoveAll<IAppCacheService>();
            services.RemoveAll<IAppAuditService>();

            services.AddSingleton<IAppCacheService, NullCacheService>();
            services.AddSingleton<IAppAuditService, NoOpAuditService>();
        });
    }

    public HttpClient CreateTenantClient(string tenantHost = DefaultTenantHost)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Tenant-Url", tenantHost);
        return client;
    }

    private sealed class NoOpAuditService : IAppAuditService
    {
        public Task AuditEventAsync(AuditEventInfo auditEventInfo) => Task.CompletedTask;

        public Task AuditErrorAsync(AuditErrorInfo auditErrorInfo) => Task.CompletedTask;
    }
}
