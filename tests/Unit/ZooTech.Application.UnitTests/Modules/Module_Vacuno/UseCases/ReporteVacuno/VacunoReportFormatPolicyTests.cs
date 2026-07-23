using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Domain.Configuration;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.ReporteVacuno;

public sealed class VacunoReportFormatPolicyTests
{
    private readonly ITenantConfigurationProvider _configuration =
        Substitute.For<ITenantConfigurationProvider>();

    [Theory]
    [InlineData("excel", ReportFileFormat.Excel)]
    [InlineData("PDF", ReportFileFormat.Pdf)]
    public async Task EnsureAllowedAsync_ReturnsConfiguredFormat(
        string requested,
        ReportFileFormat expected)
    {
        _configuration
            .GetSettingAsync(Settings.Vacunos.VacunosReporteFormatosDescarga)
            .Returns("excel,pdf");
        var sut = new VacunoReportFormatPolicy(_configuration);

        var result = await sut.EnsureAllowedAsync(requested);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task EnsureAllowedAsync_WhenTenantUsesLegacyXlsx_AllowsExcel()
    {
        _configuration
            .GetSettingAsync(Settings.Vacunos.VacunosReporteFormatosDescarga)
            .Returns("xlsx,pdf");
        var sut = new VacunoReportFormatPolicy(_configuration);

        var result = await sut.EnsureAllowedAsync("excel");

        result.Should().Be(ReportFileFormat.Excel);
    }

    [Theory]
    [InlineData("pdf", "excel")]
    [InlineData("csv", "excel,pdf")]
    [InlineData("excel", "")]
    [InlineData("excel", "excel,csv")]
    public async Task EnsureAllowedAsync_WhenConfigurationOrRequestIsInvalid_Throws(
        string requested,
        string configured)
    {
        _configuration
            .GetSettingAsync(Settings.Vacunos.VacunosReporteFormatosDescarga)
            .Returns(configured);
        var sut = new VacunoReportFormatPolicy(_configuration);

        var action = () => sut.EnsureAllowedAsync(requested);

        await action.Should().ThrowAsync<ValidationException>();
    }
}
