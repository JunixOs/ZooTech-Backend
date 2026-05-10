using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.InterfaceAdapters.Modules.Module_Animals.Controllers;
using ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.InterfaceAdapters.Modules.Module_Animals.Presenters;
using ZooTech.Infrastructure.Persistence;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ZooTech.InterfaceAdapters.IntegrationTests;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });

        // Controllers
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(AnimalsController).Assembly);

        // Application
        builder.Services.AddScoped<IListAnimalsInputPort, ListAnimalsInteractor>();
        
        // Presenter (Must be scoped because we read its state after Handle)
        builder.Services.AddScoped<ListAnimalsPresenter>();
        builder.Services.AddScoped<IListAnimalsOutputPort>(sp => sp.GetRequiredService<ListAnimalsPresenter>());

        // Infrastructure / Mocked for DB Testing
        builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        // Tenant Factory (will be overridden in test or uses header context)
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ITenantContext, TestTenantContext>();
        builder.Services.AddScoped<ITenantDbContextFactory, TestTenantDbContextFactory>();

        // Feature Service
        builder.Services.AddScoped<IFeatureService, TestFeatureService>();

        var app = builder.Build();

        app.UseRouting();

        app.Use(async (context, next) =>
        {
            var tenantCtx = context.RequestServices.GetRequiredService<ITenantContext>() as TestTenantContext;
            if (tenantCtx != null && context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdStr) && Guid.TryParse(tenantIdStr, out var tenantId))
            {
                tenantCtx.TenantId = tenantId;
            }
            await next();
        });

        app.MapControllers();

        app.Run();
    }
}

// Fakes for Infrastructure to allow testing

public class TestTenantContext : ITenantContext
{
    public Guid TenantId { get; set; } = Guid.Empty;
    public string DatabaseName => $"GanaderiaDb_{TenantId}";
}

public class TestFeatureService : IFeatureService
{
    public Task<bool> IsEnabledAsync(string featureKey) => Task.FromResult(true);
}

public class TestTenantDbContextFactory : ITenantDbContextFactory
{
    private readonly ITenantContext _tenantContext;
    private readonly IServiceProvider _serviceProvider;

    public TestTenantDbContextFactory(ITenantContext tenantContext, IServiceProvider serviceProvider)
    {
        _tenantContext = tenantContext;
        _serviceProvider = serviceProvider;
    }

    public GanaderiaDbContext CreateDbContext()
    {
        // One isolated InMemory DB per TenantId
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase($"GanaderiaDb_{_tenantContext.TenantId}")
            .Options;

        return new GanaderiaDbContext(options);
    }
}
