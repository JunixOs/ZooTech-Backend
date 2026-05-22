---
name: multi-tenancy
description: >
  Implementar arquitectura multi-tenant en aplicaciones .NET con base de datos
  por tenant, resolución por subdominio, contexto de tenant en la request,
  provisioning dinámico y catálogo de tenants. Usar cuando se diseñen sistemas
  SaaS, cuando múltiples clientes compartan la misma instancia de API pero
  con datos aislados, o cuando se necesite separación de bases de datos por tenant.
---

# Multi-Tenancy — Arquitectura por Base de Datos en .NET

## Estrategias de Multi-Tenancy

| Estrategia | Aislamiento | Costo | Cuándo usar |
|------------|-------------|-------|-------------|
| **DB por tenant** | Alto | Alto | Datos sensibles, clientes enterprise |
| **Schema por tenant** | Medio | Medio | Balance seguridad/costo |
| **Tabla compartida** | Bajo | Bajo | SaaS masivo, datos no sensibles |

**Este skill cubre: Base de Datos por Tenant** (la más robusta).

---

## Paso 1 — Modelo de datos: Catálogo de Tenants

Una base de datos central almacena el registro de todos los tenants:

```csharp
// Infrastructure/Persistence/TenantCatalogDb.cs
public class TenantCatalogDb : DbContext
{
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();

    public TenantCatalogDb(DbContextOptions<TenantCatalogDb> options)
        : base(options) { }
}

public class TenantEntity
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = default!;        // subdominio: "granja-lopez"
    public string DatabaseName { get; set; } = default!; // "Ganaderia_granja_lopez_Db"
    public bool IsActive { get; set; }
}
```

---

## Paso 2 — Contexto de Tenant (Application + Infrastructure)

**Interfaz en Application** (la capa de dominio no sabe nada de tenants):

```csharp
// Application/Common/Gateway/Context/ITenantContext.cs
public interface ITenantContext
{
    Guid TenantId { get; }
    string DatabaseName { get; }
}
```

**Implementación en Infrastructure** (almacena el tenant actual de la request):

```csharp
// Infrastructure/Tenant/TenantContext.cs
public class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }
    public string DatabaseName { get; private set; } = "";

    public void SetTenant(Guid id, string dbName)
    {
        TenantId = id;
        DatabaseName = dbName;
    }
}
```

Registro DI: `services.AddScoped<TenantContext>(); services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());`

---

## Paso 3 — Resolución del Tenant por subdominio (Middleware)

```csharp
// InterfaceAdapters/Middleware/TenantResolutionMiddleware.cs
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _baseDomain;

    public TenantResolutionMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _baseDomain = config["MultiTenant:BaseDomain"]!; // ej: "zoosoft.pe"
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantStore tenantStore,
        TenantContext tenantContext)  // TenantContext (no la interfaz, para poder llamar SetTenant)
    {
        var host = context.Request.Host.Host;   // ej: "granja-lopez.zoosoft.pe"
        var slug = ExtractSlug(host);           // ej: "granja-lopez"

        if (slug == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Tenant no encontrado");
            return;
        }

        var tenant = await tenantStore.GetBySlugAsync(slug);
        if (tenant == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Tenant inválido");
            return;
        }

        // Poblar el contexto de tenant para esta request
        tenantContext.SetTenant(tenant.Id, tenant.DatabaseName);

        await _next(context);
    }

    private string? ExtractSlug(string host)
    {
        if (!host.EndsWith("." + _baseDomain)) return null;
        return host[..^(_baseDomain.Length + 1)];
    }
}
```

**Registro en el pipeline:**

```csharp
app.UseMiddleware<TenantResolutionMiddleware>(); // antes de Auth y Controllers
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## Paso 4 — TenantStore: obtener info del tenant (con caché)

```csharp
// Infrastructure/Tenant/TenantStore.cs
public class TenantStore : ITenantStore
{
    private readonly TenantCatalogDb _db;
    private readonly IMemoryCache _cache;

    public async Task<TenantInfo?> GetBySlugAsync(string slug)
    {
        var cacheKey = $"tenant:{slug}";

        if (_cache.TryGetValue(cacheKey, out TenantInfo cached))
            return cached;

        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug);

        if (tenant == null || !tenant.IsActive)
            return null;

        var result = new TenantInfo
        {
            Id = tenant.Id,
            Slug = tenant.Slug,
            DatabaseName = tenant.DatabaseName,
            IsActive = tenant.IsActive
        };

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
```

---

## Paso 5 — Factory de DbContext por Tenant

En lugar de inyectar un `DbContext` fijo, usar una Factory que lee el tenant activo:

```csharp
// Application (interfaz)
public interface ITenantDbContextFactory
{
    GanaderiaDbContext CreateDbContext();
}

// Infrastructure (implementación)
public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly ITenantContext _tenant;
    private readonly IConfiguration _config;

    public GanaderiaDbContext CreateDbContext()
    {
        var template = _config.GetConnectionString("TenantTemplate")!;
        // Template: "Server=...;Database={DATABASE};..."
        var conn = template.Replace("{DATABASE}", _tenant.DatabaseName);

        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseSqlServer(conn)
            .Options;

        return new GanaderiaDbContext(options);
    }
}
```

**Connection string template en appsettings.json:**

```json
{
  "ConnectionStrings": {
    "TenantTemplate": "Server=localhost;Database={DATABASE};Trusted_Connection=True;"
  },
  "MultiTenant": {
    "BaseDomain": "zoosoft.pe"
  }
}
```

---

## Paso 6 — Repositorios usando la Factory

```csharp
public class AnimalRepository : IAnimalRepository
{
    private readonly ITenantDbContextFactory _factory;

    public async Task AddAsync(Animal animal)
    {
        using var db = _factory.CreateDbContext(); // BD del tenant actual
        var entity = CowMapper.ToEntity(animal);
        db.Cows.Add(entity);
        await db.SaveChangesAsync();
    }
}
```

---

## Paso 7 — Provisioning de un nuevo Tenant

```csharp
// Infrastructure/Tenant/TenantProvisioningService.cs
public class TenantProvisioningService
{
    public async Task ProvisionAsync(CreateTenantDto dto)
    {
        // 1. Generar nombre de BD
        var dbName = $"Ganaderia_{dto.Slug.Replace("-", "_")}_Db";

        // 2. Registrar en catálogo
        var tenant = new TenantEntity
        {
            Id = Guid.NewGuid(),
            Slug = dto.Slug,
            DatabaseName = dbName,
            IsActive = true
        };
        _catalogDb.Tenants.Add(tenant);
        await _catalogDb.SaveChangesAsync();

        // 3. Crear BD física y aplicar migraciones
        var conn = _template.Replace("{DATABASE}", dbName);
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseSqlServer(conn).Options;

        await using var db = new GanaderiaDbContext(options);
        await db.Database.MigrateAsync();

        // 4. Seed inicial opcional
        await SeedAsync(db);
    }
}
```

---

## Caché con clave por Tenant

Cuando uses caché en los Interactors, incluye el `TenantId` en la clave:

```csharp
// ✅ Correcto — caché aislada por tenant
var minNameLength = await _cache.GetOrCreateAsync(
    $"tenant:{_tenant.TenantId}:config:minName",
    async () => await _config.GetValueAsync<int>("animal.name.min"),
    TimeSpan.FromMinutes(5)
);

// ❌ Incorrecto — caché compartida entre tenants
var minNameLength = await _cache.GetOrCreateAsync(
    "config:minName",   // <-- sin tenant, datos de un cliente afectan a otro
    ...
);
```

---

## Manejo de Errores

| Situación | Respuesta |
|-----------|-----------|
| Slug no encontrado en host | 404 "Tenant no encontrado" |
| Tenant en catálogo pero `IsActive=false` | 404 "Tenant inválido" |
| BD del tenant caída | 500 con log detallado |
| Tenant no resuelto antes de llegar al Controller | Middleware debe abortar antes |

---

## Buenas Prácticas

- **Registrar `TenantContext` como Scoped**, no Singleton (una instancia por request).
- **Nunca inyectar `TenantContext` directamente en Domain o Application**; usar la interfaz `ITenantContext`.
- **Cachear info del tenant** en `TenantStore` (5-10 min) para evitar consultas al catálogo en cada request.
- **Siempre incluir `TenantId` en las claves de caché** de Application.
- **Auditoria incluye `TenantId`** en cada registro (ver skill `infrastructure-persistence`).
- **Migraciones automáticas** al provisionar un nuevo tenant (`MigrateAsync()`).

## Patrones Comunes

- **Slug como subdominio**: `granja-lopez.zoosoft.pe` → `slug = "granja-lopez"`.
- **Template de connection string**: un solo patrón con `{DATABASE}` como placeholder.
- **TenantStore con doble caché**: primero `IMemoryCache`, luego consulta a `TenantCatalogDb`.
- **ProvisioningService separado**: no mezclar la creación de tenants con la lógica de negocio normal.
