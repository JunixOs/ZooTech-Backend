using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

namespace ZooTech.Application.UnitTests.Modules.Animals.UseCases.ReportAnimalList;

public class ReportAnimalListInteractorTests
{
    [Fact]
    public void ValidateAndNormalize_WithoutDates_ShouldUseLastThirtyDays()
    {
        var validator = new ReportAnimalListValidator();
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var filter = validator.ValidateAndNormalize(new ReportAnimalListCommand(
            FechaInicio: null,
            FechaFin: null,
            Keyword: null,
            RazaCode: null,
            ColorCode: null,
            SexoCode: null,
            TipoAdquisicionCode: null,
            GranjaId: null,
            EstadoCode: null,
            ExportExcel: false,
            ExportPdf: false));

        Assert.Equal(today, filter.FechaFin);
        Assert.Equal(today.AddDays(-30), filter.FechaInicio);
    }

    [Fact]
    public void ValidateAndNormalize_WhenFechaInicioIsGreaterThanFechaFin_ShouldThrow()
    {
        var validator = new ReportAnimalListValidator();

        var exception = Assert.Throws<ReportAnimalListValidationException>(() =>
            validator.ValidateAndNormalize(CreateCommand(
                fechaInicio: new DateOnly(2024, 2, 1),
                fechaFin: new DateOnly(2024, 1, 1))));

        Assert.Contains("fechaInicio", exception.Errors.Keys);
    }

    [Fact]
    public void ValidateAndNormalize_WhenRangeExceedsLimit_ShouldThrow()
    {
        var validator = new ReportAnimalListValidator();

        var exception = Assert.Throws<ReportAnimalListValidationException>(() =>
            validator.ValidateAndNormalize(CreateCommand(
                fechaInicio: new DateOnly(2023, 1, 1),
                fechaFin: new DateOnly(2024, 2, 1))));

        Assert.Contains("rangoFechas", exception.Errors.Keys);
    }

    [Fact]
    public void ValidateAndNormalize_WithKeyword_ShouldTrimKeyword()
    {
        var validator = new ReportAnimalListValidator();

        var filter = validator.ValidateAndNormalize(CreateCommand(keyword: "  Lola  "));

        Assert.Equal("Lola", filter.Keyword);
    }

    [Fact]
    public void ValidateAndNormalize_WhenKeywordExceedsLimit_ShouldThrow()
    {
        var validator = new ReportAnimalListValidator();
        var keyword = new string('A', 101);

        var exception = Assert.Throws<ReportAnimalListValidationException>(() =>
            validator.ValidateAndNormalize(CreateCommand(keyword: keyword)));

        Assert.Contains("keyword", exception.Errors.Keys);
    }

    [Fact]
    public void ValidateAndNormalize_WithNewFilters_ShouldNormalizeCodes()
    {
        var validator = new ReportAnimalListValidator();

        var filter = validator.ValidateAndNormalize(CreateCommand(
            razaCode: " HOL ",
            colorCode: "NEG",
            sexoCode: " H ",
            tipoAdquisicionCode: " NAC ",
            granjaId: 8,
            estadoCode: " ACT "));

        Assert.Equal("HOL", filter.RazaCode);
        Assert.Equal("NEG", filter.ColorCode);
        Assert.Equal("H", filter.SexoCode);
        Assert.Equal("NAC", filter.TipoAdquisicionCode);
        Assert.Equal(8, filter.GranjaId);
        Assert.Equal("ACT", filter.EstadoCode);
    }

    [Fact]
    public void ValidateAndNormalize_WithInvalidGranjaId_ShouldThrow()
    {
        var validator = new ReportAnimalListValidator();

        var exception = Assert.Throws<ReportAnimalListValidationException>(() =>
            validator.ValidateAndNormalize(CreateCommand(granjaId: 0)));

        Assert.Contains("granjaId", exception.Errors.Keys);
    }

    [Fact]
    public async Task Handle_WithoutExport_ShouldPresentListOutput()
    {
        var repository = new FakeAnimalReportRepository();
        var interactor = CreateInteractor(repository);
        var outputPort = new FakeOutputPort();

        await interactor.Handle(CreateCommand(exportExcel: false, exportPdf: false), outputPort);

        Assert.NotNull(outputPort.ReceivedListOutput);
        Assert.Single(outputPort.ReceivedListOutput.Items);
        Assert.Null(outputPort.ReceivedExcelOutput);
        Assert.Null(outputPort.ReceivedPdfOutput);
    }

    [Fact]
    public async Task Handle_WithExportExcelTrue_ShouldPresentExcelOutput()
    {
        var repository = new FakeAnimalReportRepository();
        var interactor = CreateInteractor(repository);
        var outputPort = new FakeOutputPort();

        await interactor.Handle(CreateCommand(exportExcel: true, exportPdf: false), outputPort);

        Assert.NotNull(outputPort.ReceivedExcelOutput);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", outputPort.ReceivedExcelOutput.ContentType);
        Assert.Equal(new byte[] { 4, 5, 6 }, outputPort.ReceivedExcelOutput.Content);
        Assert.StartsWith("reporte-vacunos-", outputPort.ReceivedExcelOutput.FileName);
        Assert.EndsWith(".xlsx", outputPort.ReceivedExcelOutput.FileName);
    }

    [Fact]
    public async Task Handle_WithExportPdfTrue_ShouldPresentPdfOutput()
    {
        var repository = new FakeAnimalReportRepository();
        var interactor = CreateInteractor(repository);
        var outputPort = new FakeOutputPort();

        await interactor.Handle(CreateCommand(exportExcel: false, exportPdf: true), outputPort);

        Assert.NotNull(outputPort.ReceivedPdfOutput);
        Assert.Equal("application/pdf", outputPort.ReceivedPdfOutput.ContentType);
        Assert.Equal(new byte[] { 1, 2, 3 }, outputPort.ReceivedPdfOutput.Content);
        Assert.StartsWith("reporte-vacunos-", outputPort.ReceivedPdfOutput.FileName);
        Assert.EndsWith(".pdf", outputPort.ReceivedPdfOutput.FileName);
    }

    [Fact]
    public async Task Handle_WithNewFilters_ShouldSendNormalizedFiltersToRepository()
    {
        var repository = new FakeAnimalReportRepository();
        var interactor = CreateInteractor(repository);
        var outputPort = new FakeOutputPort();

        await interactor.Handle(CreateCommand(
            keyword: "  LO  ",
            razaCode: " HOL ",
            sexoCode: " H ",
            tipoAdquisicionCode: " NAC ",
            granjaId: 2,
            estadoCode: " ACT "), outputPort);

        Assert.NotNull(repository.ReceivedFilter);
        Assert.Equal("LO", repository.ReceivedFilter.Keyword);
        Assert.Equal("HOL", repository.ReceivedFilter.RazaCode);
        Assert.Equal("H", repository.ReceivedFilter.SexoCode);
        Assert.Equal("NAC", repository.ReceivedFilter.TipoAdquisicionCode);
        Assert.Equal(2, repository.ReceivedFilter.GranjaId);
        Assert.Equal("ACT", repository.ReceivedFilter.EstadoCode);
    }

    private static ReportAnimalListInteractor CreateInteractor(FakeAnimalReportRepository repository)
    {
        return new ReportAnimalListInteractor(
            repository,
            new FakeExcelService(),
            new FakePdfService(),
            new ReportAnimalListValidator());
    }

    private static ReportAnimalListCommand CreateCommand(
        DateOnly? fechaInicio = null,
        DateOnly? fechaFin = null,
        string? keyword = null,
        string? razaCode = null,
        string? colorCode = null,
        string? sexoCode = null,
        string? tipoAdquisicionCode = null,
        long? granjaId = null,
        string? estadoCode = null,
        bool exportExcel = false,
        bool exportPdf = false)
    {
        return new ReportAnimalListCommand(
            fechaInicio ?? new DateOnly(2023, 1, 1),
            fechaFin ?? new DateOnly(2023, 1, 31),
            keyword,
            razaCode,
            colorCode,
            sexoCode,
            tipoAdquisicionCode,
            granjaId,
            estadoCode,
            exportExcel,
            exportPdf);
    }

    private sealed class FakeAnimalReportRepository : IAnimalReportRepository
    {
        public ReportAnimalListFilter? ReceivedFilter { get; private set; }

        public Task<IReadOnlyCollection<ReportAnimalListItem>> GetAnimalListAsync(
            ReportAnimalListFilter filter,
            CancellationToken cancellationToken = default)
        {
            ReceivedFilter = filter;
            IReadOnlyCollection<ReportAnimalListItem> items = new List<ReportAnimalListItem>
            {
                new(
                    "V001",
                    "Lola",
                    new DateOnly(2021, 5, 10),
                    "Nacimiento",
                    "Holstein",
                    "Negro",
                    "Hembra",
                    "Granja Norte",
                    "Activo",
                    new DateOnly(2023, 1, 15))
            };
            return Task.FromResult(items);
        }
    }

    private sealed class FakeExcelService : IAnimalReportExcelService
    {
        public byte[] GenerateAnimalListExcel(ReportAnimalListOutput output) => new byte[] { 4, 5, 6 };
    }

    private sealed class FakePdfService : IAnimalReportPdfService
    {
        public byte[] GenerateAnimalListPdf(ReportAnimalListOutput output) => new byte[] { 1, 2, 3 };
    }

    private sealed class FakeOutputPort : IReportAnimalListOutputPort
    {
        public ReportAnimalListPdfOutput? ReceivedPdfOutput { get; private set; }
        public ReportAnimalListExcelOutput? ReceivedExcelOutput { get; private set; }
        public ReportAnimalListOutput? ReceivedListOutput { get; private set; }
        public ReportAnimalListValidationException? ReceivedValidationError { get; private set; }
        public AnimalReportNotFoundException? ReceivedNotFound { get; private set; }
        public AnimalReportGenerationException? ReceivedUnexpectedError { get; private set; }

        public void PresentList(ReportAnimalListOutput output) => ReceivedListOutput = output;
        public void PresentExcel(ReportAnimalListExcelOutput output) => ReceivedExcelOutput = output;
        public void PresentPdf(ReportAnimalListPdfOutput output) => ReceivedPdfOutput = output;
        public void PresentValidationError(ReportAnimalListValidationException exception) => ReceivedValidationError = exception;
        public void PresentNotFound(AnimalReportNotFoundException exception) => ReceivedNotFound = exception;
        public void PresentUnexpectedError(AnimalReportGenerationException exception) => ReceivedUnexpectedError = exception;
    }
}
