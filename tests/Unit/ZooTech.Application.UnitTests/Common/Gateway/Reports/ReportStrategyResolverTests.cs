using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Reports;

namespace ZooTech.Application.UnitTests.Common.Gateway.Reports;

public sealed class ReportStrategyResolverTests
{
    [Fact]
    public void Resolve_ReturnsTheStrategyRegisteredForTheFormat()
    {
        var excel = CreateStrategy(ReportFileFormat.Excel);
        var pdf = CreateStrategy(ReportFileFormat.Pdf);
        var sut = new ReportStrategyResolver<TestModel>([excel, pdf]);

        sut.Resolve(ReportFileFormat.Excel).Should().BeSameAs(excel);
        sut.Resolve(ReportFileFormat.Pdf).Should().BeSameAs(pdf);
    }

    [Fact]
    public void Constructor_WhenFormatIsDuplicated_Throws()
    {
        var action = () => new ReportStrategyResolver<TestModel>(
            [
                CreateStrategy(ReportFileFormat.Excel),
                CreateStrategy(ReportFileFormat.Excel)
            ]);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Resolve_WhenFormatIsMissing_Throws()
    {
        var sut = new ReportStrategyResolver<TestModel>(
            [CreateStrategy(ReportFileFormat.Excel)]);

        var action = () => sut.Resolve(ReportFileFormat.Pdf);

        action.Should().Throw<InvalidOperationException>();
    }

    private static IReportStrategy<TestModel> CreateStrategy(ReportFileFormat format)
    {
        var strategy = Substitute.For<IReportStrategy<TestModel>>();
        strategy.Format.Returns(format);
        return strategy;
    }

    public sealed record TestModel;
}
