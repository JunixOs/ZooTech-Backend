using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public sealed class ExportarActividadVacunosInteractorTests
{
    [Fact]
    public async Task HandleAsync_UsesSharedStatsAndRequestedStrategy()
    {
        var statsService = Substitute.For<IVacunoActivityStatsService>();
        var policy = Substitute.For<IVacunoReportFormatPolicy>();
        var resolver = Substitute.For<IReportStrategyResolver<ActividadVacunosReportModel>>();
        var strategy = Substitute.For<IReportStrategy<ActividadVacunosReportModel>>();
        var start = new DateOnly(2026, 7, 1);
        var end = new DateOnly(2026, 7, 3);
        var stats = new GetActivityStatsOutput(
            "2026-07-01",
            "2026-07-03",
            [new GetActivityPointOutput("2026-07-01", 10)],
            10,
            10);
        var expected = new GeneratedReportDocument(
            [1, 2],
            "application/pdf",
            "pdf",
            "Actividad_Vacunos_2026-07-01_2026-07-03.pdf");

        policy.EnsureAllowedAsync("pdf").Returns(ReportFileFormat.Pdf);
        statsService.GetAsync(start, end, Arg.Any<CancellationToken>()).Returns(stats);
        resolver.Resolve(ReportFileFormat.Pdf).Returns(strategy);
        strategy.GenerateAsync(
                Arg.Any<ActividadVacunosReportModel>(),
                Arg.Any<CancellationToken>())
            .Returns(expected);

        var sut = new ExportarActividadVacunosInteractor(statsService, policy, resolver);
        var result = await sut.HandleAsync(
            new ExportarActividadVacunosQuery(start, end, "pdf"));

        result.Should().BeSameAs(expected);
        await strategy.Received(1).GenerateAsync(
            Arg.Is<ActividadVacunosReportModel>(model => ReferenceEquals(model.Stats, stats)),
            Arg.Any<CancellationToken>());
    }
}
