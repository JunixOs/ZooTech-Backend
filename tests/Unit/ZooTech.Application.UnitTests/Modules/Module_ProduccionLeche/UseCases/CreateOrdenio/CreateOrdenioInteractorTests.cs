using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.CreateOrdenio;

public class CreateOrdenioInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenCodigoAlreadyExists_ThrowsConflictException()
    {
        var repository = new FakeOrdenioRepository
        {
            ExistsCodigoResult = true,
            ExistsVacunoResult = true,
            ExistsUsuarioResult = true,
            ExistsEstadoResult = true
        };
        var interactor = new CreateOrdenioInteractor(repository);

        var command = new CreateOrdenioCommand(
            Codigo: "ORD-001",
            FechaHora: DateTime.UtcNow,
            VacunoId: 1,
            EncargadoUsuarioId: 2,
            Litros: 10,
            EstadoOrdenioCode: "ACTIVO",
            Observaciones: null,
            ActorUsuarioId: 2);

        var action = () => interactor.HandleAsync(command, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        public bool ExistsCodigoResult { get; set; }
        public bool ExistsVacunoFechaResult { get; set; }
        public bool ExistsVacunoResult { get; set; }
        public bool ExistsUsuarioResult { get; set; }
        public bool ExistsEstadoResult { get; set; }

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult(ExistsCodigoResult);

        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoFechaResult);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoResult);

        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsUsuarioResult);

        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
            => Task.FromResult(ExistsEstadoResult);

        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

        public Task<IReadOnlyList<OrdenioOutput>> ListAsync(ListOrdeniosQuery query, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<OrdenioOutput>>(Array.Empty<OrdenioOutput>());

        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);

        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);
    }
}
