using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;
using ZooTech.Infrastructure.Persistence;
using ZooTech.Infrastructure.Persistence.Repositories;
using ZooTech.Infrastructure.Time;
using ZooTech.InterfaceAdapters.Modules.Module_Animals.Presenters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

// DI: Application
builder.Services.AddScoped<IListAnimalsInputPort, ListAnimalsInteractor>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

// DI: Interface Adapters
builder.Services.AddScoped<ListAnimalsPresenter>();
builder.Services.AddScoped<IListAnimalsOutputPort>(sp => sp.GetRequiredService<ListAnimalsPresenter>());

// DI: Infrastructure
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();

// Dummy Implementations for missing global services (Tenant, Features, DbFactory)
builder.Services.AddSingleton<ITenantContext, DummyTenantContext>();
builder.Services.AddSingleton<ITenantDbContextFactory, DummyTenantDbContextFactory>();
builder.Services.AddSingleton<IFeatureService, DummyFeatureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Dummies para poder correr sin la infraestructura completa del SaaS
public class DummyTenantContext : ITenantContext
{
    public Guid TenantId { get; } = Guid.NewGuid();
    public string DatabaseName => "GanaderiaDb_Local";
}

public class DummyFeatureService : IFeatureService
{
    public Task<bool> IsEnabledAsync(string featureKey) => Task.FromResult(true);
}

public class DummyTenantDbContextFactory : ITenantDbContextFactory
{
    public GanaderiaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase("GanaderiaDb_Local")
            .Options;
        
        var db = new GanaderiaDbContext(options);
        db.Database.EnsureCreated();
        
        // Seed if empty
        if (!db.Animals.Any())
        {
            db.Animals.Add(new ZooTech.Infrastructure.Persistence.Entities.AnimalEntity
            {
                Codigo = "LOCAL-01",
                Nombre = "Vaca Ejecucion Local",
                Estado = "VIVO",
                RazaCode = "HOL",
                RazaNombre = "Holstein",
                ProcedenciaGranja = "Mi Granja",
                ProcedenciaDistrito = "D1", ProcedenciaProvincia = "P1", ProcedenciaDepartamento = "Dep1",
                FechaNacimiento = DateTime.UtcNow.AddDays(-10),
                FechaRegistro = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        }

        return db;
    }
}
