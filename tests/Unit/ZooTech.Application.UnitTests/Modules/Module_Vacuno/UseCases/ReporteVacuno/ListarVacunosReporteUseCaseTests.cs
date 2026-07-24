using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.ReporteVacuno;

public sealed class ListarVacunosReporteUseCaseTests
{
    private readonly IListadoVacunosReporteReadRepository _repository =
        Substitute.For<IListadoVacunosReporteReadRepository>();
    private readonly IReportStrategyResolver<ListadoVacunosReportModel> _resolver =
        Substitute.For<IReportStrategyResolver<ListadoVacunosReportModel>>();
    private readonly IReportStrategy<ListadoVacunosReportModel> _strategy =
        Substitute.For<IReportStrategy<ListadoVacunosReportModel>>();
    private readonly IVacunoReportFormatPolicy _formatPolicy =
        Substitute.For<IVacunoReportFormatPolicy>();
    private readonly IReportFileStorage _storage =
        Substitute.For<IReportFileStorage>();

    [Fact]
    public async Task HandleAsync_Json_LoadsOnlyRequestedPage()
    {
        _repository.ListarAsync(Arg.Any<ListadoVacunosReporteReadQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ListadoVacunosReporteReadResult([CreateItem(1)], 1));

        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateQuery("json", "2", "25"));

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(25);
        result.DownloadUrl.Should().BeNull();
        await _repository.Received(1).ListarAsync(
            Arg.Is<ListadoVacunosReporteReadQuery>(query => query.Page == 2 && query.Limit == 25),
            Arg.Any<CancellationToken>());
        _resolver.DidNotReceiveWithAnyArgs().Resolve(default);
        await _formatPolicy.DidNotReceiveWithAnyArgs().EnsureAllowedAsync(default);
        await _storage.DidNotReceiveWithAnyArgs().SaveAsync(
            default!, default!, default!, default!, default);
    }

    [Fact]
    public async Task HandleAsync_Excel_LoadsAllPagesInBatchesOfOneHundred()
    {
        var firstPage = Enumerable.Range(1, 100).Select(index => CreateItem(index)).ToList();
        var secondPage = Enumerable.Range(101, 1).Select(index => CreateItem(index)).ToList();

        _repository.ListarAsync(
                Arg.Is<ListadoVacunosReporteReadQuery>(query => query.Page == 1 && query.Limit == 10),
                Arg.Any<CancellationToken>())
            .Returns(new ListadoVacunosReporteReadResult([CreateItem(1)], 101));
        _repository.ListarAsync(
                Arg.Is<ListadoVacunosReporteReadQuery>(query => query.Page == 1 && query.Limit == 100),
                Arg.Any<CancellationToken>())
            .Returns(new ListadoVacunosReporteReadResult(firstPage, 101));
        _repository.ListarAsync(
                Arg.Is<ListadoVacunosReporteReadQuery>(query => query.Page == 2 && query.Limit == 100),
                Arg.Any<CancellationToken>())
            .Returns(new ListadoVacunosReporteReadResult(secondPage, 101));
        _formatPolicy.EnsureAllowedAsync("excel").Returns(ReportFileFormat.Excel);
        _resolver.Resolve(ReportFileFormat.Excel).Returns(_strategy);
        _strategy.GenerateAsync(
                Arg.Any<ListadoVacunosReportModel>(),
                Arg.Any<CancellationToken>())
            .Returns(new GeneratedReportDocument(
                [1, 2, 3],
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlsx",
                "Listado_Vacunos.xlsx"));
        _storage.SaveAsync(
                "listado_vacunos",
                "xlsx",
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<CancellationToken>())
            .Returns(new StoredReportFile("listado.xlsx", "/descargas/listado.xlsx"));

        var sut = CreateSut();
        var result = await sut.HandleAsync(CreateQuery("excel"));

        result.Data.Should().ContainSingle();
        result.TotalCount.Should().Be(101);
        result.DownloadUrl.Should().Be("/descargas/listado.xlsx");
        await _repository.Received(3)
            .ListarAsync(Arg.Any<ListadoVacunosReporteReadQuery>(), Arg.Any<CancellationToken>());
        await _strategy.Received(1).GenerateAsync(
            Arg.Is<ListadoVacunosReportModel>(model => model.Items.Count == 101),
            Arg.Any<CancellationToken>());
        await _storage.Received(1).SaveAsync(
            "listado_vacunos",
            "xlsx",
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<CancellationToken>());
    }

    private ListarVacunosReporteUseCase CreateSut()
        => new(_repository, _resolver, _formatPolicy, _storage);

    private static ListarVacunosReporteQuery CreateQuery(
        string formato,
        string? page = null,
        string? pageSize = null)
        => new(
            null, null, null, null, null, null, null, null, null,
            null, null, null, formato, page, pageSize, null);

    private static VacunoListadoReporteReadItem CreateItem(int id)
        => new(
            id,
            $"VAC-{id}",
            new DateOnly(2020, 1, 1),
            new DateOnly(2023, 1, 1),
            $"Vacuno {id}",
            "Compra",
            "Holstein",
            "Negro",
            "Hembra",
            "Granja",
            "Procedencia",
            "vivo",
            "activo");
}
