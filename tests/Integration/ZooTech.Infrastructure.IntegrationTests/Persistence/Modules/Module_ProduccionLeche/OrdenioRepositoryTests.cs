using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

namespace ZooTech.Infrastructure.IntegrationTests.Persistence.Modules.Module_ProduccionLeche;

public class OrdenioRepositoryTests
{
    [Fact]
    public async Task ListAsync_AppliesFiltersPagingAndProjectsNames()
    {
        await using var dbContext = CreateDbContext();
        SeedOrdenios(dbContext);
        var repository = new OrdenioRepository(dbContext);

        var result = await repository.ListAsync(
            vacunoId: 1,
            estadoOrdenioCode: "ACTIVO",
            fechaDesde: new DateTime(2026, 7, 1, 0, 0, 0),
            fechaHasta: new DateTime(2026, 7, 3, 23, 59, 59),
            page: 1,
            pageSize: 1,
            CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("ORD-002", item.Codigo);
        Assert.Equal("Luna", item.NombreVacuno);
        Assert.Equal("Juan Perez", item.NombreCompleto);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrdenioIsDeleted_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();
        SeedOrdenios(dbContext);
        var repository = new OrdenioRepository(dbContext);

        var result = await repository.GetByIdAsync(4, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsCodigoAsync_TrimsCodeAndIgnoresDeletedRows()
    {
        await using var dbContext = CreateDbContext();
        SeedOrdenios(dbContext);
        var repository = new OrdenioRepository(dbContext);

        var existing = await repository.ExistsCodigoAsync(" ORD-001 ", CancellationToken.None);
        var deleted = await repository.ExistsCodigoAsync("ORD-004", CancellationToken.None);

        Assert.True(existing);
        Assert.False(deleted);
    }

    private static GanaderiaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GanaderiaDbContext(options);
    }

    private static void SeedOrdenios(GanaderiaDbContext dbContext)
    {
        var now = new DateTime(2026, 7, 7, 10, 0, 0);
        var vacunoLuna = new vacuno
        {
            id = 1,
            codigo = "VAC-001",
            nombre = "Luna",
            fecha_nacimiento = new DateOnly(2020, 1, 1),
            tipo_adquisicion_code = "COMPRA",
            raza_code = "HOLSTEIN",
            color_code = "NEGRO",
            sexo_code = "H",
            granja_id = 1,
            fecha_registro = new DateOnly(2026, 1, 1),
            created_at = now,
            updated_at = now
        };
        var vacunoEstrella = new vacuno
        {
            id = 2,
            codigo = "VAC-002",
            nombre = "Estrella",
            fecha_nacimiento = new DateOnly(2021, 1, 1),
            tipo_adquisicion_code = "COMPRA",
            raza_code = "HOLSTEIN",
            color_code = "NEGRO",
            sexo_code = "H",
            granja_id = 1,
            fecha_registro = new DateOnly(2026, 1, 1),
            created_at = now,
            updated_at = now
        };
        var encargado = new usuario
        {
            id = 10,
            codigo = "USR-001",
            nombre_usuario = "jperez",
            nombre_completo = "Juan Perez",
            activo = true,
            created_at = now,
            updated_at = now
        };

        dbContext.ordenios.AddRange(
            CreateOrdenio(1, "ORD-001", new DateTime(2026, 7, 1, 8, 0, 0), vacunoLuna, encargado, "ACTIVO", null),
            CreateOrdenio(2, "ORD-002", new DateTime(2026, 7, 2, 8, 0, 0), vacunoLuna, encargado, "ACTIVO", null),
            CreateOrdenio(3, "ORD-003", new DateTime(2026, 7, 3, 8, 0, 0), vacunoEstrella, encargado, "ACTIVO", null),
            CreateOrdenio(4, "ORD-004", new DateTime(2026, 7, 4, 8, 0, 0), vacunoLuna, encargado, "ACTIVO", now),
            CreateOrdenio(5, "ORD-005", new DateTime(2026, 7, 2, 9, 0, 0), vacunoLuna, encargado, "ANULADO", null));

        dbContext.SaveChanges();
    }

    private static ordenio CreateOrdenio(
        long id,
        string codigo,
        DateTime fechaHora,
        vacuno vacuno,
        usuario encargado,
        string estadoOrdenioCode,
        DateTime? deletedAt)
        => new()
        {
            id = id,
            codigo = codigo,
            fecha_hora = fechaHora,
            vacuno_id = vacuno.id,
            vacuno = vacuno,
            encargado_usuario_id = encargado.id,
            encargado_usuario = encargado,
            litros = 12.5m,
            estado_ordenio_code = estadoOrdenioCode,
            observaciones = null,
            created_at = fechaHora,
            updated_at = fechaHora,
            deleted_at = deletedAt
        };
}
