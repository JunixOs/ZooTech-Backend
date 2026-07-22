using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;

namespace ZooTech.Infrastructure.UnitTests.Common.Services.PdfGenerator;

public class PdfGeneratorServiceTests
{
    [Fact]
    public void GenerateOrdeniosReport_WithEmptyItems_ReturnsPdfBytes()
    {
        var service = new PdfGeneratorService();
        var document = new GenerateOrdeniosPdfDocument(
            Array.Empty<OrdenioListOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc));

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithEmptyItems_ReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var document = new GenerateOrdeniosPdfDocument(
            Array.Empty<OrdenioListOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc));

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithoutVacunoId_ComparesVacunosAndReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithVacunoId_UsesVacunoHistoryAndReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt, 1, "Luna", "VAC-001", 2, "Juan Perez", 14, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: 1,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    // ── Phase 1: Logic Layer RED tests (tasks 1.1–1.3) ──

    [Fact]
    public void ComputeDailySeries_EmptyInput_ReturnsEmpty()
    {
        var result = PdfGenerateComparationService.ComputeDailySeries(Array.Empty<OrdenioListOutput>());

        result.Should().BeEmpty();
    }

    [Fact]
    public void ComputeDailySeries_SingleRecord_ReturnsSingleAggregate()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date)
        };

        var result = PdfGenerateComparationService.ComputeDailySeries(items);

        result.Should().HaveCount(1);
        var day = result[0];
        day.DateLabel.Should().Be("01/07/2026");
        day.Sum.Should().Be(10f);
        day.Average.Should().Be(10f);
    }

    [Fact]
    public void ComputeDailySeries_MultipleVacunosSameDate_AveragesPerDate()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date),
            new OrdenioListOutput(2, "ORD-002", date, 2, "Estrella", "VAC-002", 2, "Maria", 6m, "ACTIVO", null, date, date)
        };

        var result = PdfGenerateComparationService.ComputeDailySeries(items);

        result.Should().HaveCount(1);
        var day = result[0];
        day.DateLabel.Should().Be("01/07/2026");
        day.Sum.Should().Be(16f);       // 10 + 6
        day.Average.Should().Be(8f);    // (10 + 6) / 2
    }

    [Fact]
    public void ComputeDailySeries_MultipleDates_ReturnsMultipleDaysSorted()
    {
        var day1 = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", day1, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, day1, day1),
            new OrdenioListOutput(2, "ORD-002", day2, 2, "Estrella", "VAC-002", 2, "Maria", 14m, "ACTIVO", null, day2, day2)
        };

        var result = PdfGenerateComparationService.ComputeDailySeries(items);

        result.Should().HaveCount(2);
        result[0].DateLabel.Should().Be("01/07/2026");
        result[0].Sum.Should().Be(10f);
        result[1].DateLabel.Should().Be("03/07/2026");
        result[1].Sum.Should().Be(14f);
    }

    // ── Phase 2: Chart Rewrite RED tests (tasks 2.1–2.3) ──

    // ── Phase 3: Fix actual series to use individual production values (not daily sums) ──

    [Fact]
    public void ComputeActualSeries_MultipleRecords_ReturnsIndividualPoints_NotAggregated()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date),
            new OrdenioListOutput(2, "ORD-002", date, 2, "Estrella", "VAC-002", 2, "Maria", 6m, "ACTIVO", null, date, date)
        };

        var result = PdfGenerateComparationService.ComputeActualSeries(items);

        // Must return 2 individual points (NOT 1 aggregated point)
        result.Should().HaveCount(2);
        result[0].Value.Should().Be(10f);
        result[0].Timestamp.Should().Be(date);
        result[1].Value.Should().Be(6f);
        result[1].Timestamp.Should().Be(date);
    }

    [Fact]
    public void ComputeActualSeries_SingleRecord_ReturnsOnePoint()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date)
        };

        var result = PdfGenerateComparationService.ComputeActualSeries(items);

        result.Should().HaveCount(1);
        result[0].Timestamp.Should().Be(date);
        result[0].Value.Should().Be(10f);
        result[0].Label.Should().Be("01/07/2026 08:00");
    }

    [Fact]
    public void ComputeActualSeries_Empty_ReturnsEmpty()
    {
        var result = PdfGenerateComparationService.ComputeActualSeries(Array.Empty<OrdenioListOutput>());

        result.Should().BeEmpty();
    }

    [Fact]
    public void ComputeActualSeries_MultipleDates_OrdersByDateTime()
    {
        var day1 = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc);
        // Insert items out of order to prove ordering
        var items = new[]
        {
            new OrdenioListOutput(2, "ORD-002", day2, 2, "Estrella", "VAC-002", 2, "Maria", 14m, "ACTIVO", null, day2, day2),
            new OrdenioListOutput(1, "ORD-001", day1, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, day1, day1)
        };

        var result = PdfGenerateComparationService.ComputeActualSeries(items);

        result.Should().HaveCount(2);
        result[0].Timestamp.Should().Be(day1);
        result[0].Value.Should().Be(10f); // day1 comes first after ordering
        result[1].Timestamp.Should().Be(day2);
        result[1].Value.Should().Be(14f); // day2 comes second
    }

    [Fact]
    public void ComputeReferenceSeries_MultipleVacunosSameDate_ReturnsDailyAverage()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date),
            new OrdenioListOutput(2, "ORD-002", date, 2, "Estrella", "VAC-002", 2, "Maria", 6m, "ACTIVO", null, date, date)
        };

        var result = PdfGenerateComparationService.ComputeReferenceSeries(items);

        result.Should().HaveCount(1);
        result[0].Timestamp.Should().Be(date.Date);
        result[0].Value.Should().Be(8f); // (10 + 6) / 2 = 8
        result[0].Label.Should().Be("01/07/2026");
    }

    [Fact]
    public void ComputeReferenceSeries_Empty_ReturnsEmpty()
    {
        var result = PdfGenerateComparationService.ComputeReferenceSeries(Array.Empty<OrdenioListOutput>());

        result.Should().BeEmpty();
    }

    [Fact]
    public void ComputeReferenceSeries_TwoDates_ReturnsOnePointPerDate()
    {
        var day1 = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", day1, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, day1, day1),
            new OrdenioListOutput(2, "ORD-002", day2, 2, "Estrella", "VAC-002", 2, "Maria", 14m, "ACTIVO", null, day2, day2)
        };

        var result = PdfGenerateComparationService.ComputeReferenceSeries(items);

        result.Should().HaveCount(2);
        result[0].Timestamp.Should().Be(day1.Date);
        result[0].Value.Should().Be(10f);
        result[1].Timestamp.Should().Be(day2.Date);
        result[1].Value.Should().Be(14f);
    }

    [Fact]
    public void CalculateTimelineX_SameTimestamp_ReturnsSameCoordinateForBothSeries()
    {
        var min = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var max = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc);
        var timestamp = new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc);

        var actualX = PdfGenerateComparationService.CalculateTimelineX(timestamp, min, max, 55, 690);
        var referenceX = PdfGenerateComparationService.CalculateTimelineX(timestamp, min, max, 55, 690);

        actualX.Should().Be(referenceX);
        actualX.Should().BeApproximately(400, 0.01f);
    }

    [Fact]
    public void CalculateTimelineX_SingleTimestamp_CentersThePoint()
    {
        var timestamp = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var x = PdfGenerateComparationService.CalculateTimelineX(timestamp, timestamp, timestamp, 55, 690);

        x.Should().Be(400);
    }

    [Fact]
    public void GenerateOrdeniosReport_MultiVacuno_ActualSeriesIsIndividualProduction_NotDailySum()
    {
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        // Two vacunos on same date: Luna 10L + Estrella 14L = 24L sum, but individual values are 10 and 14
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-2), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var svg = PdfGenerateComparationService.BuildChartSvg(document, 800, 360);

        svg.Should().NotBeNullOrEmpty();

        // The y-axis max should be based on individual values (max=14), NOT daily sum (max=24)
        // Extract y-axis labels from the SVG (they are text elements drawn near the left edge)
        var yLabelMatches = Regex.Matches(svg, @"<text[^>]*>\s*(\d+)\s*</text>");
        var yValues = yLabelMatches
            .Select(m => int.Parse(m.Groups[1].Value))
            .Where(v => v > 0)
            .ToList();

        // With individual max=14, y-axis ceil should be ~15 (rounded to 5).
        // With old daily-sum max=24, y-axis ceil would be 25.
        // The highest y-axis label tells us the scale.
        var maxYLabel = yValues.Max();
        maxYLabel.Should().BeLessThanOrEqualTo(15,
            $"y-axis max label {maxYLabel} suggests daily sums are being used; individual max should scale to ~15");
    }

    [Fact]
    public void GenerateOrdeniosReport_MultiVacuno_Empty_ReturnsPdfWithEmptyMessage()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            Array.Empty<OrdenioListOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateOrdeniosReport_MultiVacunoMultiDate_ReturnsNonEmptyPdf()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-2), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(4, "ORD-004", generatedAt.AddDays(-1), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 12, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(5, "ORD-005", generatedAt, 1, "Luna", "VAC-001", 2, "Juan Perez", 12, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(1000);
    }

    [Fact]
    public void GenerateOrdeniosReport_MultiVacuno_NotGroupedByVacuno()
    {
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-2), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(4, "ORD-004", generatedAt.AddDays(-1), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 12, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var svg = PdfGenerateComparationService.BuildChartSvg(document, 800, 360);

        svg.Should().NotBeNullOrEmpty();
        // Must NOT contain bar-chart <rect> elements (only background <rect fill="white"> is ok)
        var rectCount = Regex.Matches(svg, "<rect").Count;
        rectCount.Should().BeLessThanOrEqualTo(1,
            "only the background white rect from canvas.Clear is expected; bar chart would have many <rect> elements");
        if (rectCount == 1)
            svg.Should().Contain("<rect fill=\"white\"",
                "the only <rect> should be the background fill from canvas.Clear");
        // Must contain multiple <path> elements (dual line series: purple dashed + maroon)
        var pathCount = Regex.Matches(svg, "<path").Count;
        pathCount.Should().BeGreaterThanOrEqualTo(2,
            "dual-series line chart must render at least two line paths");
    }
}
