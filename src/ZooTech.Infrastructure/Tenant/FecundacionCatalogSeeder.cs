using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Tenant;

public sealed class FecundacionCatalogSeeder : IFecundacionCatalogSeeder
{
    private static readonly CatalogState[] RequiredStates =
    [
        new("EN_ESPERA", "En Espera", "Fecundación pendiente de confirmación"),
        new("GESTANTE", "Gestante", "Fecundación confirmada con gestación"),
        new("VACIA", "Vacía", "Fecundación confirmada sin gestación")
    ];

    private readonly IGanaderiaDbContextFactory _ganaderiaDbContextFactory;

    public FecundacionCatalogSeeder(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _ganaderiaDbContextFactory = ganaderiaDbContextFactory;
    }

    public async Task<FecundacionCatalogSeedResult> SeedAsync(
        string databaseName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        await using var context = _ganaderiaDbContextFactory
            .CreateDbContextBySpecificDatabaseName(databaseName, useAdminLogin: true);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var requiredCodes = RequiredStates.Select(state => state.Code).ToArray();
        var existingCodes = await context.cat_estado_fecundacion_vacunos
            .AsNoTracking()
            .Where(state => requiredCodes.Contains(state.code))
            .Select(state => state.code)
            .ToListAsync(cancellationToken);
        var existing = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = RequiredStates.Where(state => !existing.Contains(state.Code)).ToArray();

        if (missing.Length > 0)
        {
            context.cat_estado_fecundacion_vacunos.AddRange(
                missing.Select(state => new cat_estado_fecundacion_vacuno
                {
                    code = state.Code,
                    nombre = state.Name,
                    descripcion = state.Description
                }));
            await context.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return new FecundacionCatalogSeedResult(databaseName, missing.Length);
    }

    private sealed record CatalogState(string Code, string Name, string Description);
}

public sealed class TenantFecundacionCatalogBackfillService
    : ITenantFecundacionCatalogBackfillService
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IFecundacionCatalogSeeder _catalogSeeder;
    private readonly ILogger<TenantFecundacionCatalogBackfillService> _logger;

    public TenantFecundacionCatalogBackfillService(
        ITenantDbContextFactory tenantDbContextFactory,
        IFecundacionCatalogSeeder catalogSeeder,
        ILogger<TenantFecundacionCatalogBackfillService> logger)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _catalogSeeder = catalogSeeder;
        _logger = logger;
    }

    public async Task<TenantFecundacionCatalogBackfillResult> BackfillAsync(
        CancellationToken cancellationToken = default)
    {
        await using var catalog = _tenantDbContextFactory.CreateDbContextBySettingsValue();
        var tenants = await catalog.tenants
            .AsNoTracking()
            .Where(tenant =>
                tenant.deleted_at == null &&
                tenant.status != TenantStatus.INACTIVE.ToString() &&
                tenant.tenant_database_connection != null &&
                tenant.tenant_database_connection.is_active &&
                tenant.tenant_database_connection.deleted_at == null)
            .Select(tenant => new
            {
                tenant.code,
                tenant.tenant_database_connection!.database_name
            })
            .ToListAsync(cancellationToken);

        var results = new List<TenantFecundacionCatalogBackfillItem>(tenants.Count);
        foreach (var tenant in tenants)
        {
            try
            {
                var result = await _catalogSeeder.SeedAsync(
                    tenant.database_name,
                    cancellationToken);
                results.Add(new TenantFecundacionCatalogBackfillItem(
                    tenant.code,
                    tenant.database_name,
                    result.InsertedCount,
                    true,
                    null));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "No se pudo completar el catálogo de fecundación para el tenant {TenantCode}.",
                    tenant.code);
                results.Add(new TenantFecundacionCatalogBackfillItem(
                    tenant.code,
                    tenant.database_name,
                    0,
                    false,
                    "No se pudo completar el catálogo."));
            }
        }

        return new TenantFecundacionCatalogBackfillResult(results);
    }
}
