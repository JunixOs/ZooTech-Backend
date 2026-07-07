using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.GetOrdenioById;

public class GetOrdenioByIdInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenOrdenioExists_ReturnsMappedData()
    {
        var fecha = new DateTime(2026, 6, 18, 8, 0, 0, DateTimeKind.Utc);
        var repository = new FakeOrdenioRepository(CreateOrdenio(fecha));
        var interactor = new GetOrdenioByIdInteractor(repository);

        var result = await interactor.HandleAsync(10, CancellationToken.None);

        Assert.Equal(10, result.Data.Id);
        Assert.Equal("ORD-010", result.Data.Codigo);
        Assert.Equal("Luna", result.Data.NombreVacuno);
        Assert.Equal("ACTIVO", result.Data.EstadoOrdenioCode);
    }

    [Fact]
    public async Task HandleAsync_WhenOrdenioDoesNotExist_ThrowsNotFoundException()
    {
        var repository = new FakeOrdenioRepository(null);
        var interactor = new GetOrdenioByIdInteractor(repository);

        await Assert.ThrowsAsync<NotFoundException>(() => interactor.HandleAsync(99, CancellationToken.None));
    }

    private static Ordenio CreateOrdenio(DateTime fecha)
        => Ordenio.Rehydrate(
            id: 10,
            codigo: "ORD-010",
            fechaHora: fecha,
            vacunoId: 1,
            nombreVacuno: "Luna",
            encargadoUsuarioId: 2,
            nombreCompleto: "Juan Perez",
            litros: 12,
            estadoOrdenioCode: "ACTIVO",
            observaciones: "Inicial",
            createdAt: fecha,
            updatedAt: fecha,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 2,
            updatedBy: 2,
            deletedBy: null);

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        private readonly Ordenio? _ordenio;

        public FakeOrdenioRepository(Ordenio? ordenio)
        {
            _ordenio = ordenio;
        }

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken) => Task.FromResult(_ordenio);
        public Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken cancellationToken) => Task.FromResult<(IReadOnlyList<OrdenioList> Items, int TotalCount)>((Array.Empty<OrdenioList>(), 0));
        public Task<IReadOnlyList<OrdenioList>> ListReportAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<OrdenioList>>(Array.Empty<OrdenioList>());
        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken) => Task.FromResult(ordenio);
        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken) => Task.FromResult(ordenio);
    }
}
