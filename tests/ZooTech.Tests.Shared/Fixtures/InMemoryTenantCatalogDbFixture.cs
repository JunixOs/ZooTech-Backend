using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Tests.Shared.Fixtures;

public class InMemoryTenantCatalogDbFixture : IDisposable
{
    public TenantCatalogDb Context { get; }

    public InMemoryTenantCatalogDbFixture()
    {
        var options = new DbContextOptionsBuilder<TenantCatalogDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new TenantCatalogDb(options);
    }

    public void Dispose() => Context.Dispose();
}
