using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

namespace ZooTech.Application.UnitTests.ReporteVacuno;

[TestClass]
public sealed class ListarReporteVacunosUseCaseTests
{
    [TestMethod]
    public async Task HandleAsync_WhenNoDates_ShouldSendDefaultRangeToRepository()
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        await useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "json", null, null));

        Assert.IsNotNull(repository.LastCriteria);
        Assert.AreEqual(new DateOnly(2026, 4, 28), repository.LastCriteria!.FechaDesde);
        Assert.AreEqual(new DateOnly(2026, 5, 28), repository.LastCriteria.FechaHasta);
        Assert.AreEqual(1, repository.LastCriteria.Page);
        Assert.AreEqual(20, repository.LastCriteria.Limit);
    }

    [TestMethod]
    public async Task HandleAsync_WhenKeywordAndFiltersAreSent_ShouldNormalizeAndForwardCriteria()
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        var response = await useCase.HandleAsync(new ListarReporteVacunosQuery(
            "2026-05-01", "2026-05-28", "  Luna  ", " Angus ", " Granja Norte ", " VIVO ", " PRODUCCION_LECHE ", " JSON ", "2", "25"));

        var criteria = repository.LastCriteria!;
        Assert.AreEqual("luna", criteria.Q);
        Assert.AreEqual("angus", criteria.Raza);
        Assert.AreEqual("granja norte", criteria.Procedencia);
        Assert.AreEqual("vivo", criteria.Estado);
        Assert.AreEqual("produccion_leche", criteria.AptoPara);
        Assert.AreEqual(2, criteria.Page);
        Assert.AreEqual(25, criteria.Limit);
        Assert.AreEqual("luna", response.Filtros.Q);
        Assert.AreEqual("angus", response.Filtros.Raza);
        Assert.AreEqual("granja norte", response.Filtros.Procedencia);
        Assert.AreEqual("vivo", response.Filtros.Estado);
        Assert.AreEqual("produccion_leche", response.Filtros.AptoPara);
        Assert.AreEqual("json", response.Filtros.Formato);
    }

    [TestMethod]
    public async Task HandleAsync_WhenLimitExceedsMaximum_ShouldCapLimitAtOneHundred()
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        await useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "json", "1", "500"));

        Assert.AreEqual(100, repository.LastCriteria!.Limit);
    }

    [TestMethod]
    [DataRow(null, null, 1, 20)]
    [DataRow("", "", 1, 20)]
    [DataRow("   ", "   ", 1, 20)]
    [DataRow("0", "20", 1, 20)]
    [DataRow("1", "0", 1, 20)]
    public async Task HandleAsync_WhenPaginationIsMissingOrZero_ShouldUseDefaults(string? page, string? limit, int expectedPage, int expectedLimit)
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        await useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "json", page, limit));

        Assert.AreEqual(expectedPage, repository.LastCriteria!.Page);
        Assert.AreEqual(expectedLimit, repository.LastCriteria.Limit);
    }

    [TestMethod]
    [DataRow("-1", "20", "page")]
    [DataRow("1", "-10", "limit")]
    [DataRow("abc", "20", "page")]
    [DataRow("1", "abc", "limit")]
    public async Task HandleAsync_WhenPaginationIsInvalid_ShouldThrowValidationError(string page, string limit, string expectedField)
    {
        var useCase = CreateUseCase(new FakeReporteVacunoReadRepository());

        var ex = await Assert.ThrowsExactlyAsync<ApplicationRuleException>(() =>
            useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "json", page, limit)));

        Assert.AreEqual("VALIDATION_ERROR", ex.Code);
        Assert.IsTrue(ex.Details.Any(x => x.Field == expectedField));
    }

    [TestMethod]
    [DataRow("activo", "estado")]
    [DataRow("leche", "aptoPara")]
    [DataRow("word", "formato")]
    public async Task HandleAsync_WhenEnumFilterIsInvalid_ShouldThrowExpectedError(string value, string field)
    {
        var useCase = CreateUseCase(new FakeReporteVacunoReadRepository());
        var query = field switch
        {
            "estado" => new ListarReporteVacunosQuery(null, null, null, null, null, value, null, "json", null, null),
            "aptoPara" => new ListarReporteVacunosQuery(null, null, null, null, null, null, value, "json", null, null),
            _ => new ListarReporteVacunosQuery(null, null, null, null, null, null, null, value, null, null)
        };

        var ex = await Assert.ThrowsExactlyAsync<ApplicationRuleException>(() => useCase.HandleAsync(query));

        Assert.AreEqual(field == "formato" ? "INVALID_REPORT_FORMAT" : "VALIDATION_ERROR", ex.Code);
        Assert.IsTrue(ex.Details.Any(x => x.Field == field));
    }

    [TestMethod]
    public async Task HandleAsync_WhenFormatIsExcel_ShouldGenerateDownloadUrl()
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        var response = await useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "excel", null, null));

        Assert.AreEqual("excel", response.Filtros.Formato);
        Assert.AreEqual("/reportes/vacunos/reporte_listado_vacunos_20260528.xlsx", response.DownloadUrl);
    }

    [TestMethod]
    public async Task HandleAsync_WhenFormatIsPdf_ShouldGenerateDownloadUrl()
    {
        var repository = new FakeReporteVacunoReadRepository();
        var useCase = CreateUseCase(repository);

        var response = await useCase.HandleAsync(new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "pdf", null, null));

        Assert.AreEqual("pdf", response.Filtros.Formato);
        Assert.AreEqual("/reportes/vacunos/reporte_listado_vacunos_20260528.pdf", response.DownloadUrl);
    }

    private static ListarReporteVacunosUseCase CreateUseCase(FakeReporteVacunoReadRepository repository)
    {
        return new ListarReporteVacunosUseCase(
            repository,
            new FakeListadoVacunosReportFileService(),
            new FixedDateTimeProvider(new DateOnly(2026, 5, 28)),
            NullLogger<ListarReporteVacunosUseCase>.Instance);
    }

    private sealed class FakeListadoVacunosReportFileService : IListadoVacunosReportFileService
    {
        public Task<ListadoVacunosReportFileResult> GenerateExcelAsync(
            IReadOnlyCollection<VacunoListadoItem> data,
            ReporteVacunoResumen resumen,
            ReporteVacunoFiltros filtros,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ListadoVacunosReportFileResult(
                "reporte_listado_vacunos_20260528.xlsx",
                "/reportes/vacunos/reporte_listado_vacunos_20260528.xlsx"));
        }

        public Task<ListadoVacunosReportFileResult> GeneratePdfAsync(
            IReadOnlyCollection<VacunoListadoItem> data,
            ReporteVacunoResumen resumen,
            ReporteVacunoFiltros filtros,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ListadoVacunosReportFileResult(
                "reporte_listado_vacunos_20260528.pdf",
                "/reportes/vacunos/reporte_listado_vacunos_20260528.pdf"));
        }
    }

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public FixedDateTimeProvider(DateOnly today) => Today = today;
        public DateOnly Today { get; }
    }

    private sealed class FakeReporteVacunoReadRepository : IReporteVacunoReadRepository
    {
        public ReporteVacunoListadoCriteria? LastCriteria { get; private set; }

        public Task<ReporteVacunoListadoPage> ListarAsync(ReporteVacunoListadoCriteria criteria, CancellationToken cancellationToken = default)
        {
            LastCriteria = criteria;
            return Task.FromResult(new ReporteVacunoListadoPage(Array.Empty<VacunoListadoItem>(), 0));
        }
    }
}
