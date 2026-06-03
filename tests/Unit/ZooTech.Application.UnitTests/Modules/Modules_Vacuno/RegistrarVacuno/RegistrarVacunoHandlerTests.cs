using System.Reflection;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;
using ZooTech.Domain.Exceptions;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.RegistrarVacuno;

public sealed class RegistrarVacunoHandlerTests
{
    [Fact]
    public async Task Handle_CodigoDuplicado_LanzaExcepcionYNoGuarda()
    {
        var repository = new FakeVacunoRepository { Existe = true };
        var archivo = new FakeArchivoService();
        var handler = new RegistrarVacunoHandler(repository, archivo, new FakeTimeProvider());

        await Assert.ThrowsAsync<VacunoYaExisteException>(
            () => handler.Handle(RegistrarVacunoValidatorTests.ComandoValido(), CancellationToken.None));

        Assert.False(repository.Guardo);
        Assert.False(archivo.Guardo);
    }

    [Fact]
    public async Task Handle_CodigoUnico_GuardaRegistro()
    {
        var repository = new FakeVacunoRepository();
        var handler = new RegistrarVacunoHandler(repository, new FakeArchivoService(), new FakeTimeProvider());

        var result = await handler.Handle(RegistrarVacunoValidatorTests.ComandoValido(), CancellationToken.None);

        Assert.True(repository.Guardo);
        Assert.Equal(10, result.Id);
        Assert.Equal("VACA001", result.Codigo);
    }

    [Fact]
    public async Task Handle_ConFoto_GuardaArchivoYAsociaRuta()
    {
        var archivo = new FakeArchivoService();
        var command = RegistrarVacunoValidatorTests.ComandoValido() with
        {
            FotoStream = new MemoryStream([1, 2, 3]),
            FotoNombreOriginal = "vaca.jpg"
        };
        var handler = new RegistrarVacunoHandler(new FakeVacunoRepository(), archivo, new FakeTimeProvider());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(archivo.Guardo);
        Assert.Equal("vacunos/fotos/vaca.jpg", result.FotoUrl);
    }

    private sealed class FakeVacunoRepository : IVacunoRepository
    {
        public bool Existe { get; init; }
        public bool Guardo { get; private set; }

        public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default) => Task.FromResult(Existe);

        public Task<Vacuno> RegistrarAsync(
            Vacuno vacuno,
            VacunoAdquisicion adquisicion,
            VacunoUtilizacionHistorial utilizacion,
            VacunoFoto? foto,
            CancellationToken ct = default)
        {
            Guardo = true;
            typeof(Vacuno).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)!
                .SetValue(vacuno, 10);
            return Task.FromResult(vacuno);
        }

        public Task<IReadOnlyList<Vacuno>> ListarAsync(int page, int limit, string? q, string? estado, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Vacuno>>([]);

        public Task<int> ContarAsync(string? q, string? estado, CancellationToken ct = default) => Task.FromResult(0);

        public Task<Vacuno?> ObtenerPorIdAsync(int id, CancellationToken ct = default) => Task.FromResult<Vacuno?>(null);

        public Task<Vacuno?> ObtenerPorCodigoAsync(string codigo, CancellationToken ct = default) => Task.FromResult<Vacuno?>(null);

        public Task<bool> ExisteRegistroDuplicadoAsync(
            string codigoExcluido,
            EditarVacunoData data,
            CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<Vacuno?> ActualizarAsync(
            string codigo,
            EditarVacunoData data,
            CancellationToken ct = default)
            => Task.FromResult<Vacuno?>(null);

        public Task<IReadOnlyList<UbigeoOption>> ListarUbigeoAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<UbigeoOption>>([]);
    }

    private sealed class FakeArchivoService : IArchivoService
    {
        public bool Guardo { get; private set; }

        public Task<string> GuardarAsync(Stream stream, string nombreOriginal, string carpeta, CancellationToken ct = default)
        {
            Guardo = true;
            return Task.FromResult($"{carpeta}/{nombreOriginal}");
        }
    }

    private sealed class FakeTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => new(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
    }
}
