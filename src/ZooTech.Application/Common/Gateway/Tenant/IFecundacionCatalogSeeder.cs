namespace ZooTech.Application.Common.Gateway.Tenant;

public interface IFecundacionCatalogSeeder
{
    Task<FecundacionCatalogSeedResult> SeedAsync(
        string databaseName,
        CancellationToken cancellationToken = default);
}

public sealed record FecundacionCatalogSeedResult(
    string DatabaseName,
    int InsertedCount);

public interface ITenantFecundacionCatalogBackfillService
{
    Task<TenantFecundacionCatalogBackfillResult> BackfillAsync(
        CancellationToken cancellationToken = default);
}

public sealed record TenantFecundacionCatalogBackfillItem(
    string TenantCode,
    string DatabaseName,
    int InsertedCount,
    bool Success,
    string? Error);

public sealed record TenantFecundacionCatalogBackfillResult(
    IReadOnlyList<TenantFecundacionCatalogBackfillItem> Items)
{
    public bool Success => Items.All(item => item.Success);
}
