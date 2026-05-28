using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.API;

namespace ZooTech.API.IntegrationTests.Support;

public sealed class ReporteVacunoApiFactory : WebApplicationFactory<Program>
{
    private readonly bool _forceDownloadFailure;

    public ReporteVacunoApiFactory(bool forceDownloadFailure = false)
    {
        _forceDownloadFailure = forceDownloadFailure;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var values = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\MSSQLLocalDB;Database=ZooTechTest;Trusted_Connection=True;TrustServerCertificate=True",
                ["Swagger:Enabled"] = "false"
            };
            config.AddInMemoryCollection(values);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IReporteVacunoReadRepository>();
            services.RemoveAll<IRegistroVacunoReadRepository>();
            services.RemoveAll<IDateTimeProvider>();

            services.AddSingleton<IDateTimeProvider>(new FixedDateTimeProvider(new DateOnly(2026, 5, 28)));
            services.AddSingleton<TestSeedData>();
            services.AddScoped<IReporteVacunoReadRepository, SeededReporteVacunoReadRepository>();
            services.AddScoped<IRegistroVacunoReadRepository, SeededRegistroVacunoReadRepository>();

            if (_forceDownloadFailure)
            {
                services.RemoveAll<IRegistroVacunoExcelReportService>();
                services.AddScoped<IRegistroVacunoExcelReportService, FailingExcelReportService>();
            }
        });
    }
}

internal sealed class FixedDateTimeProvider : IDateTimeProvider
{
    public FixedDateTimeProvider(DateOnly today) => Today = today;
    public DateOnly Today { get; }
}

internal sealed class TestSeedData
{
    public IReadOnlyList<VacunoListadoItem> Listado { get; } =
    [
        new VacunoListadoItem(1, "VACA001", new DateOnly(2026, 5, 20), "Luna", "Angus", "Granja Norte", "vivo"),
        new VacunoListadoItem(2, "VACA002", new DateOnly(2026, 3, 10), "Estrella", "Holstein", "Granja Sur", "muerto")
    ];

    public RegistroVacunoDetalle Detalle { get; } = new(
        1,
        "VACA001",
        "Luna",
        new DateOnly(2022, 6, 10),
        "compra",
        1500.50m,
        "Angus",
        "Negro",
        "hembra",
        "TORO001",
        "VACA000",
        null,
        null,
        "Granja Norte",
        "Rupa-Rupa",
        "Leoncio Prado",
        "Huanuco",
        "Proveedor X",
        "produccion_leche",
        new DateOnly(2024, 3, 15),
        "Registro inicial",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "VIVO",
        "Vivo",
        "vivo",
        new DateOnly(2024, 3, 15),
        "Alta inicial",
        new DateOnly(2024, 3, 15),
        new DateOnly(2024, 3, 15),
        "admin",
        new DateTime(2024, 3, 15, 8, 30, 0),
        "admin",
        new DateTime(2024, 3, 16, 9, 0, 0));
}

internal sealed class SeededReporteVacunoReadRepository : IReporteVacunoReadRepository
{
    private readonly TestSeedData _seed;

    public SeededReporteVacunoReadRepository(TestSeedData seed) => _seed = seed;

    public Task<ReporteVacunoListadoPage> ListarAsync(ReporteVacunoListadoCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = _seed.Listado.AsEnumerable();
        query = query.Where(x => x.FechaRegistro >= criteria.FechaDesde && x.FechaRegistro <= criteria.FechaHasta);

        if (!string.IsNullOrWhiteSpace(criteria.Q))
        {
            query = query.Where(x =>
                x.Codigo.Contains(criteria.Q, StringComparison.OrdinalIgnoreCase) ||
                x.Nombre.Contains(criteria.Q, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Raza))
        {
            query = query.Where(x => (x.Raza ?? string.Empty).Contains(criteria.Raza, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Procedencia))
        {
            query = query.Where(x => (x.Procedencia ?? string.Empty).Contains(criteria.Procedencia, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Estado))
        {
            query = query.Where(x => string.Equals(x.Estado, criteria.Estado, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query.OrderByDescending(x => x.FechaRegistro).ToList();
        var paged = filtered
            .Skip((criteria.Page - 1) * criteria.Limit)
            .Take(criteria.Limit)
            .ToList();

        return Task.FromResult(new ReporteVacunoListadoPage(paged, filtered.Count));
    }
}

internal sealed class SeededRegistroVacunoReadRepository : IRegistroVacunoReadRepository
{
    private readonly TestSeedData _seed;

    public SeededRegistroVacunoReadRepository(TestSeedData seed) => _seed = seed;

    public Task<RegistroVacunoDetalle?> ObtenerRegistroAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(vacunoId == _seed.Detalle.Id ? _seed.Detalle : null);
    }
}

internal sealed class FailingExcelReportService : IRegistroVacunoExcelReportService
{
    public Task<RegistroVacunoExcelReportResult> GenerateAsync(RegistroVacunoDetalle vacuno, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Fallo simulado durante generación de Excel.");
    }
}
