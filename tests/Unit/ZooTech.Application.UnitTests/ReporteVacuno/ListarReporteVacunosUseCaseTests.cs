using Microsoft.Extensions.Logging.Abstractions;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

namespace ZooTech.Application.UnitTests.ReporteVacuno;

public sealed class ListarReporteVacunosUseCaseTests
{
    [Fact]
    public async Task HandleAsync_WhenDatesAreMissing_AppliesLastThirtyDays()
    {
        var repository = new FakeRepository();
        var useCase = CreateUseCase(repository);

        await useCase.HandleAsync(new ListarReporteVacunosQuery(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null));

        Assert.NotNull(repository.LastCriteria);
        Assert.Equal(new DateOnly(2026, 4, 20), repository.LastCriteria!.FechaDesde);
        Assert.Equal(new DateOnly(2026, 5, 20), repository.LastCriteria.FechaHasta);
        Assert.Equal(1, repository.LastCriteria.Page);
        Assert.Equal(20, repository.LastCriteria.Limit);
    }

    [Fact]
    public async Task HandleAsync_WhenFechaDesdeIsGreaterThanFechaHasta_ThrowsValidationError()
    {
        var useCase = CreateUseCase(new FakeRepository());

        var exception = await Assert.ThrowsAsync<ApplicationRuleException>(() =>
            useCase.HandleAsync(new ListarReporteVacunosQuery(
                "2026-05-21",
                "2026-05-20",
                null,
                null,
                null,
                null,
                null,
                "json",
                null,
                null)));

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Contains(exception.Details, detail => detail.Field == "fechaDesde");
    }

    [Fact]
    public async Task HandleAsync_WhenFormatoIsInvalid_ThrowsInvalidReportFormat()
    {
        var useCase = CreateUseCase(new FakeRepository());

        var exception = await Assert.ThrowsAsync<ApplicationRuleException>(() =>
            useCase.HandleAsync(new ListarReporteVacunosQuery(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                "word",
                null,
                null)));

        Assert.Equal("INVALID_REPORT_FORMAT", exception.Code);
        Assert.Contains(exception.Details, detail => detail.Field == "formato");
    }

    private static ListarReporteVacunosUseCase CreateUseCase(FakeRepository repository)
    {
        return new ListarReporteVacunosUseCase(
            repository,
            new FixedDateTimeProvider(),
            NullLogger<ListarReporteVacunosUseCase>.Instance);
    }

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public DateOnly Today => new(2026, 5, 20);
    }

    private sealed class FakeRepository : IReporteVacunoReadRepository
    {
        public ReporteVacunoListadoCriteria? LastCriteria { get; private set; }

        public Task<ReporteVacunoListadoPage> ListarAsync(
            ReporteVacunoListadoCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            LastCriteria = criteria;
            return Task.FromResult(new ReporteVacunoListadoPage([], 0));
        }
    }
}
