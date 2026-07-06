using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.GenerateOrdeniosPdf;

public class GenerateOrdeniosPdfInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenComparativoIsFalse_UsesStandardGeneratorAndStandardFileName()
    {
        var now = new DateTime(2026, 7, 2, 10, 11, 12, DateTimeKind.Utc);
        var repository = new FakeOrdenioRepository();
        var standardGenerator = new FakePdfGeneratorService(new byte[] { 1, 2, 3 });
        var comparativeGenerator = new FakeComparationPdfGeneratorService(new byte[] { 9, 9, 9 });
        var interactor = new GenerateOrdeniosPdfInteractor(
            repository,
            standardGenerator,
            comparativeGenerator,
            new FakeDateTimeProvider(now));

        var result = await interactor.HandleAsync(
            new GenerateOrdeniosComparationPdfQuery(1, "ACTIVO", now.AddDays(-1), now, false),
            CancellationToken.None);

        Assert.True(standardGenerator.WasCalled);
        Assert.False(comparativeGenerator.WasCalled);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal("reporte-ordenios-20260702101112.pdf", result.FileName);
        Assert.Equal(new byte[] { 1, 2, 3 }, result.Content);
        Assert.Equal(1, repository.CapturedVacunoId);
        Assert.Equal("ACTIVO", repository.CapturedEstadoOrdenioCode);
    }

    [Fact]
    public async Task HandleAsync_WhenComparativoIsTrue_UsesComparativeGeneratorAndComparativeFileName()
    {
        var now = new DateTime(2026, 7, 2, 10, 11, 12, DateTimeKind.Utc);
        var repository = new FakeOrdenioRepository();
        var standardGenerator = new FakePdfGeneratorService(new byte[] { 1, 2, 3 });
        var comparativeGenerator = new FakeComparationPdfGeneratorService(new byte[] { 7, 8, 9 });
        var interactor = new GenerateOrdeniosPdfInteractor(
            repository,
            standardGenerator,
            comparativeGenerator,
            new FakeDateTimeProvider(now));

        var result = await interactor.HandleAsync(
            new GenerateOrdeniosComparationPdfQuery(null, null, null, null, true),
            CancellationToken.None);

        Assert.False(standardGenerator.WasCalled);
        Assert.True(comparativeGenerator.WasCalled);
        Assert.Equal("reporte-comparativo-ordenios-20260702101112.pdf", result.FileName);
        Assert.Equal(new byte[] { 7, 8, 9 }, result.Content);
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime serverNow)
        {
            ServerNow = serverNow;
        }

        public DateTime ServerNow { get; }
    }

    private sealed class FakePdfGeneratorService : IPdfGeneratorService
    {
        private readonly byte[] _content;

        public FakePdfGeneratorService(byte[] content)
        {
            _content = content;
        }

        public bool WasCalled { get; private set; }

        public byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document)
        {
            WasCalled = true;
            return _content;
        }

        public byte[] GenerateTriajesReport(GenerateTriajesPdfDocument document)
            => throw new NotSupportedException();
    }

    private sealed class FakeComparationPdfGeneratorService : IOrdeniosComparationPdfGeneratorService
    {
        private readonly byte[] _content;

        public FakeComparationPdfGeneratorService(byte[] content)
        {
            _content = content;
        }

        public bool WasCalled { get; private set; }

        public byte[] GenerateOrdeniosReport(GenerateOrdeniosPdfDocument document)
        {
            WasCalled = true;
            return _content;
        }
    }

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        public long? CapturedVacunoId { get; private set; }
        public string? CapturedEstadoOrdenioCode { get; private set; }

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

        public Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken cancellationToken)
            => Task.FromResult<(IReadOnlyList<OrdenioList> Items, int TotalCount)>((Array.Empty<OrdenioList>(), 0));

        public Task<IReadOnlyList<OrdenioList>> ListReportAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken)
        {
            CapturedVacunoId = vacunoId;
            CapturedEstadoOrdenioCode = estadoOrdenioCode;

            var fecha = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
            var ordenio = OrdenioList.Rehydrate(
                id: 1,
                codigo: "ORD-001",
                fechaHora: fecha,
                vacunoId: 1,
                nombreVacuno: "Luna",
                encargadoUsuarioId: 2,
                nombreCompleto: "Juan Perez",
                litros: 12,
                estadoOrdenioCode: "ACTIVO",
                observaciones: null,
                createdAt: fecha,
                updatedAt: fecha,
                deletedAt: null,
                motivoEliminacion: null);

            return Task.FromResult<IReadOnlyList<OrdenioList>>(new[] { ordenio });
        }

        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);

        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);
    }
}
