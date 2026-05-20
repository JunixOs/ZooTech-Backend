---
name: infrastructure-persistence
description: >
  Implementar la capa de Infrastructure en .NET: EF Core DbContext, entidades ORM,
  repositorios, mappers dominio↔ORM, configuraciones Fluent API, auditoría automática,
  caché con IMemoryCache, servicios de configuración y features por tenant,
  motor de reglas dinámicas, proveedores de tiempo e identidad, y clientes externos.
  Usar cuando se configure acceso a base de datos, se implementen repositorios,
  se necesite auditoría de cambios o se integren servicios externos en C#/.NET.
---

# Infrastructure & Persistence — EF Core, Auditoría y Servicios en .NET

## Rol de Infrastructure Layer

Infrastructure **implementa** las interfaces definidas en Application. No define contratos, solo los cumple. Puede conocer EF Core, Redis, HttpClient, etc.

```
Application (define interfaces)  ←  Infrastructure (implementa interfaces)
```

---

## Paso 1 — DbContext principal por Tenant

```csharp
// Infrastructure/Persistence/GanaderiaDbContext.cs
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Entities;

namespace Infrastructure.Persistence;

public class GanaderiaDbContext : DbContext
{
    private readonly AuditSaveChangesInterceptor _audit;

    public DbSet<CowEntity> Cows => Set<CowEntity>();
    // Agregar un DbSet por cada entidad ORM

    public GanaderiaDbContext(
        DbContextOptions<GanaderiaDbContext> options,
        AuditSaveChangesInterceptor audit)
        : base(options)
    {
        _audit = audit;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_audit);  // registrar interceptor de auditoría
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar todas las configuraciones Fluent API del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GanaderiaDbContext).Assembly);
    }
}
```

---

## Paso 2 — Entidades ORM (≠ entidades de dominio)

Las entidades ORM son clases planas para EF Core. **Nunca son las mismas que las del dominio.**

```csharp
// Infrastructure/Persistence/Entities/CowEntity.cs
namespace Infrastructure.Persistence.Entities;

public class CowEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Infrastructure/Persistence/Entities/AppSettingEntity.cs
public class AppSettingEntity
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}

// Infrastructure/Persistence/Entities/FeatureEntity.cs
public class FeatureEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsEnabled { get; set; }
}
```

---

## Paso 3 — Configuraciones Fluent API

```csharp
// Infrastructure/Persistence/Configurations/CowConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CowConfiguration : IEntityTypeConfiguration<CowEntity>
{
    public void Configure(EntityTypeBuilder<CowEntity> builder)
    {
        builder.ToTable("Cows");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(200);

        // Índice en Status para filtros frecuentes
        builder.HasIndex(x => x.Status);
    }
}
```

---

## Paso 4 — Mappers: Dominio ↔ ORM

Cada entidad tiene su Mapper estático:

```csharp
// Infrastructure/Persistence/Mappers/CowMapper.cs
using Domain.Entities;
using Infrastructure.Persistence.Entities;

public static class CowMapper
{
    // Domain → ORM (para guardar en BD)
    public static CowEntity ToEntity(Animal animal)
    {
        return new CowEntity
        {
            Id = animal.Id,
            Name = animal.Name,
            BirthDate = animal.BirthDate,
            Status = animal.Status.ToString(),
            CreatedBy = animal.CreatedBy,
            CreatedAt = animal.CreatedAt
        };
    }

    // ORM → Domain (para devolver desde BD)
    public static Animal ToDomain(CowEntity entity)
    {
        // Usar método de reconstitución (distinto al Factory Create)
        return Animal.Reconstitute(
            entity.Id,
            entity.Name,
            entity.BirthDate,
            entity.CreatedAt,
            entity.CreatedBy,
            Enum.Parse<AnimalStatus>(entity.Status)
        );
    }
}
```

---

## Paso 5 — Repositorios

```csharp
// Infrastructure/Persistence/Repositories/AnimalRepository.cs
public class AnimalRepository : IAnimalRepository
{
    private readonly ITenantDbContextFactory _factory;

    public AnimalRepository(ITenantDbContextFactory factory)
    {
        _factory = factory;
    }

    public async Task AddAsync(Animal animal)
    {
        using var db = _factory.CreateDbContext();
        var entity = CowMapper.ToEntity(animal);
        db.Cows.Add(entity);
        await db.SaveChangesAsync();
    }

    public async Task<Animal?> GetByIdAsync(Guid id)
    {
        using var db = _factory.CreateDbContext();
        var entity = await db.Cows
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : CowMapper.ToDomain(entity);
    }

    public async Task<List<Animal>> GetAllAsync()
    {
        using var db = _factory.CreateDbContext();
        var entities = await db.Cows
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(CowMapper.ToDomain).ToList();
    }
}
```

---

## Paso 6 — Auditoría Automática con SaveChanges Interceptor

```csharp
// Infrastructure/Auditing/AuditSaveChangesInterceptor.cs
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenant;
    private readonly IRequestContext _request;

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var ctx = eventData.Context!;

        foreach (var entry in ctx.ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            // Evitar auditar la tabla de auditoría (bucle infinito)
            if (entry.Entity is AuditEntry)
                continue;

            var audit = new AuditEntry
            {
                Id = Guid.NewGuid(),
                TenantId = _tenant.TenantId,
                UserId = _request.UserId,
                Entity = entry.Entity.GetType().Name,
                Operation = entry.State.ToString(),   // Added | Modified | Deleted
                Timestamp = DateTime.UtcNow
            };

            ctx.Set<AuditEntry>().Add(audit);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

---

## Paso 7 — Servicios de Infraestructura

### ConfigurationService — Configuraciones dinámicas por tenant

```csharp
public class ConfigurationService : IConfigurationService
{
    private readonly ITenantDbContextFactory _factory;

    public async Task<T?> GetValueAsync<T>(string key)
    {
        using var db = _factory.CreateDbContext();
        var setting = db.Set<AppSettingEntity>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Key == key);

        if (setting == null) return default;
        return (T)Convert.ChangeType(setting.Value, typeof(T));
    }
}
```

### FeatureService — Feature flags por tenant

```csharp
public class FeatureService : IFeatureService
{
    private readonly ITenantDbContextFactory _factory;

    public async Task<bool> IsEnabledAsync(string feature)
    {
        using var db = _factory.CreateDbContext();
        var result = await db.Set<FeatureEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Name == feature);

        return result?.IsEnabled ?? false;  // por defecto: deshabilitado
    }
}
```

### RuleEngineService — Reglas dinámicas

```csharp
public class RuleEngineService : IRuleEngineService
{
    public Task<bool> EvaluateAsync(string rule, object input)
    {
        // Ejemplo: regla de producción baja de leche
        if (rule == "milk.low.production")
        {
            dynamic data = input;
            return Task.FromResult((decimal)data.Liters < 10m);
        }

        return Task.FromResult(true);  // por defecto: regla cumplida
    }
}
```

### AppCacheService — Caché con IMemoryCache

```csharp
public class AppCacheService : IAppCacheService
{
    private readonly IMemoryCache _cache;

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration)
    {
        if (_cache.TryGetValue(key, out T value))
            return value;

        value = await factory();
        _cache.Set(key, value, expiration);
        return value;
    }
}
```

### RequestContext — Datos de la request HTTP

```csharp
public class RequestContext : IRequestContext
{
    private readonly IHttpContextAccessor _http;

    public string? UserId =>
        _http.HttpContext?.User?.FindFirst("sub")?.Value;

    public string? UserEmail =>
        _http.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? CorrelationId =>
        _http.HttpContext?.TraceIdentifier;

    public string? Path =>
        _http.HttpContext?.Request?.Path.Value;
}
```

### DateTimeProvider — Abstracción de tiempo

```csharp
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
```

### SenasaClient — Cliente externo (ejemplo)

```csharp
// Infrastructure/External/SenasaClient.cs
public class SenasaClient
{
    private readonly HttpClient _http;

    public SenasaClient(HttpClient http)
    {
        _http = http;
    }

    public async Task SendAsync(object data)
    {
        await _http.PostAsJsonAsync("/senasa/registro", data);
    }
}
```

---

## Paso 8 — Registro de dependencias (DependencyInjection.cs)

```csharp
// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // DbContext catálogo
        services.AddDbContext<TenantCatalogDb>(opt =>
            opt.UseSqlServer(config.GetConnectionString("CatalogDb")));

        // Tenant
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantStore, TenantStore>();
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

        // Repositorios
        services.AddScoped<IAnimalRepository, AnimalRepository>();

        // Servicios de gateway
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<IRuleEngineService, RuleEngineService>();
        services.AddScoped<IAppCacheService, AppCacheService>();
        services.AddScoped<IRequestContext, RequestContext>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Auditoría
        services.AddScoped<AuditSaveChangesInterceptor>();

        // Caché en memoria
        services.AddMemoryCache();

        // Clientes externos
        services.AddHttpClient<SenasaClient>(client =>
            client.BaseAddress = new Uri(config["External:SenasaUrl"]!));

        return services;
    }
}
```

---

## Manejo de Errores

| Situación | Cómo manejar |
|-----------|-------------|
| BD del tenant no disponible | Dejar subir `SqlException`; Middleware la convierte en 500 |
| Entidad no encontrada en repositorio | Devolver `null`; el Interactor lanza `NotFoundException` |
| Error en cliente externo | Atrapar `HttpRequestException`, loguear y lanzar `BusinessException` |
| Timeout en caché | `IMemoryCache` no expira por timeout; configurar con `AbsoluteExpirationRelativeToNow` |
| Auditoría en bucle | Verificar `if (entry.Entity is AuditEntry) continue;` |

---

## Buenas Prácticas

- **Nunca inyectar DbContext directamente en repositorios**; usar `ITenantDbContextFactory`.
- **`AsNoTracking()`** en todas las consultas de lectura para mejor rendimiento.
- **Separar entidades ORM de entidades de dominio** — nunca compartir la misma clase.
- **Mappers estáticos** para conversiones dominio↔ORM (simples, sin dependencias).
- **`using var db = _factory.CreateDbContext()`** — crear y desechar el contexto por operación.
- **Configuraciones Fluent API en clases separadas** (`IEntityTypeConfiguration<T>`), no en `OnModelCreating`.
- **Clientes externos con `HttpClientFactory`** — nunca instanciar `HttpClient` manualmente.
- **Feature flags con default `false`**: si la feature no está en BD, se considera deshabilitada.

## Patrones Comunes

- **Factory Pattern para DbContext**: `ITenantDbContextFactory` devuelve el contexto del tenant activo.
- **Interceptor de auditoría**: captura automáticamente todo cambio sin modificar los repositorios.
- **Config + Cache + Feature flags**: trío estándar de configuración dinámica por tenant.
- **Repository devuelve entidades de dominio**: Infrastructure traduce ORM→Dominio antes de devolver.
