using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

namespace ZooTech.Infrastructure.IntegrationTests.Persistence.Modules.Module_Sanidad;

public class TriajeRepositoryTests
{
    private static readonly DateTime Now = new(2026, 7, 22, 10, 0, 0);

    [Fact]
    public async Task GetAllAsync_AppliesFiltersPagingAndExcludesDeletedRows()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetAllAsync(
            pagina: 1,
            tamano: 1,
            fechaDesde: "2026-07-01",
            fechaHasta: "2026-07-03",
            codigo: "TRI",
            nombre: "Luna",
            tipoPeso: "CONTROL",
            pesoKg: "121",
            vacunoId: 1,
            cancellationToken: CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
        Assert.Equal("TRI002", item.Codigo);
        Assert.Equal("Luna", item.VacunoNombre);
        Assert.Equal(121m, item.PesoKg);
    }

    [Fact]
    public async Task GetAllAsync_AppliesPesoKgPartialFilter()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetAllAsync(1, 10, pesoKg: "12");

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, item => Assert.Contains("12", item.PesoKg.ToString(System.Globalization.CultureInfo.InvariantCulture)));
    }

    [Fact]
    public async Task GetAllAsync_AppliesFechaExacta()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetAllAsync(1, 10, fecha: "2026-07-02");

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, item => Assert.Equal(new DateTime(2026, 7, 2), item.FechaHora.Date));
    }

    [Fact]
    public async Task GetAllAsync_WhenUniqueVacunoTrue_ReturnsLatestTriajePerVacuno()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetAllAsync(1, 10, uniqueVacuno: true);

        Assert.Equal(2, result.Total);
        Assert.Contains(result.Items, item => item.VacunoId == 1 && item.Codigo == "TRI003");
        Assert.Contains(result.Items, item => item.VacunoId == 2 && item.Codigo == "TRI004");
    }

    [Fact]
    public async Task GetAllAsync_OrdersByFechaHoraDescending()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetAllAsync(1, 10);

        var codigos = result.Items.Select(item => item.Codigo).ToList();
        Assert.Equal(new[] { "TRI003", "TRI004", "TRI002", "TRI001" }, codigos);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTriajeIsDeleted_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GetByIdAsync(5);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistorialByVacunoIdAsync_FiltersDatesAndOrdersAscending()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = (await repository.GetHistorialByVacunoIdAsync(1, "2026-07-02", "2026-07-03")).ToList();

        Assert.Equal(new[] { 2L, 3L }, result.Select(item => item.Id));
        Assert.True(result[0].FechaHora < result[1].FechaHora);
    }

    [Fact]
    public async Task GetHistorialGeneralAsync_FiltersDatesAndOrdersAscending()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = (await repository.GetHistorialGeneralAsync("2026-07-02", "2026-07-02")).ToList();

        Assert.Equal(new[] { 2L, 4L }, result.Select(item => item.Id));
        Assert.All(result, item => Assert.Equal(new DateTime(2026, 7, 2), item.FechaHora.Date));
    }

    [Fact]
    public async Task GetDetallesByVacunoIdAsync_ProjectsTipoPesoNameAndOrdersDescending()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = (await repository.GetDetallesByVacunoIdAsync(1)).ToList();

        Assert.Equal(new[] { "TRI003", "TRI002", "TRI001" }, result.Select(item => item.CodigoRegistro));
        Assert.Equal("Peso Final", result[0].TipoPesoMedido);
        Assert.Equal(130m, result[0].PesoKg);
    }

    [Fact]
    public async Task GenerateCodigoAsync_ReturnsNextTriCode()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        var result = await repository.GenerateCodigoAsync();

        Assert.Equal("TRI006", result);
    }

    [Fact]
    public async Task ExistsMethods_ReturnExpectedValues()
    {
        await using var dbContext = CreateDbContext();
        SeedTriajes(dbContext);
        var repository = CreateRepository(dbContext);

        Assert.True(await repository.ExistsVacunoAsync(1));
        Assert.False(await repository.ExistsVacunoAsync(99));
        Assert.True(await repository.ExistsUsuarioAsync(10));
        Assert.False(await repository.ExistsUsuarioAsync(99));
        Assert.True(await repository.ExistsTipoPesoAsync("CONTROL"));
        Assert.False(await repository.ExistsTipoPesoAsync("NO_EXISTE"));
    }

    private static GanaderiaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GanaderiaDbContext(options);
    }

    private static TriajeRepository CreateRepository(GanaderiaDbContext dbContext)
        => new(new FakeGanaderiaDbContextFactory(dbContext), new FakeDateTimeProvider(Now));

    private static void SeedTriajes(GanaderiaDbContext dbContext)
    {
        var control = new cat_tipo_peso { code = "CONTROL", nombre = "Peso Control", activo = true };
        var final = new cat_tipo_peso { code = "FINAL", nombre = "Peso Final", activo = true };
        var activo = new cat_estado_registro { code = "ACTIVO", nombre = "Activo" };
        var usuario = new usuario
        {
            id = 10,
            codigo = "USR001",
            nombre_usuario = "jperez",
            nombre_completo = "Juan Perez",
            activo = true,
            created_at = Now,
            updated_at = Now,
        };
        var luna = CreateVacuno(1, "VAC001", "Luna");
        var estrella = CreateVacuno(2, "VAC002", "Estrella");

        dbContext.cat_tipo_pesos.AddRange(control, final);
        dbContext.cat_estado_registros.Add(activo);
        dbContext.usuarios.Add(usuario);
        dbContext.vacunos.AddRange(luna, estrella);
        dbContext.triajes.AddRange(
            CreateTriaje(1, "TRI001", new DateTime(2026, 7, 1, 8, 0, 0), luna, control, activo, usuario, 120m, null),
            CreateTriaje(2, "TRI002", new DateTime(2026, 7, 2, 8, 0, 0), luna, control, activo, usuario, 121m, null),
            CreateTriaje(3, "TRI003", new DateTime(2026, 7, 3, 8, 0, 0), luna, final, activo, usuario, 130m, null),
            CreateTriaje(4, "TRI004", new DateTime(2026, 7, 2, 9, 0, 0), estrella, control, activo, usuario, 110m, null),
            CreateTriaje(5, "TRI005", new DateTime(2026, 7, 4, 8, 0, 0), luna, control, activo, usuario, 122m, Now));
        dbContext.SaveChanges();
    }

    private static vacuno CreateVacuno(long id, string codigo, string nombre)
        => new()
        {
            id = id,
            codigo = codigo,
            nombre = nombre,
            fecha_nacimiento = new DateOnly(2020, 1, 1),
            tipo_adquisicion_code = "COMPRA",
            raza_code = "HOLSTEIN",
            color_code = "NEGRO",
            sexo_code = "H",
            granja_id = 1,
            fecha_registro = new DateOnly(2026, 1, 1),
            created_at = Now,
            updated_at = Now,
        };

    private static triaje CreateTriaje(
        long id,
        string codigo,
        DateTime fechaHora,
        vacuno vacuno,
        cat_tipo_peso tipoPeso,
        cat_estado_registro estado,
        usuario usuario,
        decimal pesoKg,
        DateTime? deletedAt)
        => new()
        {
            id = id,
            codigo = codigo,
            fecha_hora = fechaHora,
            vacuno_id = vacuno.id,
            vacuno = vacuno,
            tipo_peso_code = tipoPeso.code,
            tipo_peso_codeNavigation = tipoPeso,
            peso_kg = pesoKg,
            observaciones = "Sin observaciones",
            estado_registro_code = estado.code,
            estado_registro_codeNavigation = estado,
            encargado_usuario_id = usuario.id,
            encargado_usuario = usuario,
            created_at = fechaHora,
            updated_at = fechaHora,
            deleted_at = deletedAt,
        };

    private sealed class FakeGanaderiaDbContextFactory : IGanaderiaDbContextFactory
    {
        private readonly GanaderiaDbContext _dbContext;

        public FakeGanaderiaDbContextFactory(GanaderiaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GanaderiaDbContext CreateDbContextByTenantContext() => _dbContext;

        public GanaderiaDbContext CreateDbContextBySpecificDatabaseName(string databaseName, bool useAdminLogin = false) => _dbContext;
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime serverNow)
        {
            ServerNow = serverNow;
        }

        public DateTime ServerNow { get; }
    }
}
