using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Repositories;

namespace ZooTech.Infrastructure.UnitTests.Reports;

public class AnimalReportRepositoryTests
{
    [Fact]
    public async Task GetAnimalListAsync_WithRazaCode_ShouldFilterByRaza()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(razaCode: "HOL"));

        Assert.Single(items);
        Assert.Equal("V001", items.Single().Codigo);
    }

    [Fact]
    public async Task GetAnimalListAsync_withColorCode_ShouldFilterByColorCode() {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(colorCode: "CAF"));

        Assert.Single(items);
        Assert.Equal("VOO2", items.Single().Codigo);
    }

        [Fact]
    public async Task GetAnimalListAsync_WithSexoCode_ShouldFilterBySexo()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(sexoCode: "M"));

        Assert.Single(items);
        Assert.Equal("V002", items.Single().Codigo);
    }

    [Fact]
    public async Task GetAnimalListAsync_WithTipoAdquisicionCode_ShouldFilterByTipoAdquisicion()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(tipoAdquisicionCode: "COMPRA"));

        Assert.Single(items);
        Assert.Equal("V002", items.Single().Codigo);
    }

    [Fact]
    public async Task GetAnimalListAsync_WithGranjaId_ShouldFilterByGranja()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(granjaId: 2));

        Assert.Single(items);
        Assert.Equal("V002", items.Single().Codigo);
    }

    [Fact]
    public async Task GetAnimalListAsync_WithEstadoCode_ShouldFilterByEstado()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(estadoCode: "INACT"));

        Assert.Single(items);
        Assert.Equal("V002", items.Single().Codigo);
    }

    [Fact]
    public async Task GetAnimalListAsync_WithCombinedFilters_ShouldApplyAllFilters()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(
            keyword: "Lola",
            razaCode: "HOL",
            colorCode: "NEG", 
            sexoCode: "H",
            tipoAdquisicionCode: "NAC",
            granjaId: 1,
            estadoCode: "ACT"));

        Assert.Single(items);
        var item = items.Single();
        Assert.Equal("V001", item.Codigo);
        Assert.Equal(new DateOnly(2021, 5, 10), item.FechaNacimiento);
        Assert.Equal("Nacimiento", item.TipoAdquisicion);
        Assert.Equal("Holstein", item.Raza);
        Assert.Equal("Negro", item.Color);
        Assert.Equal("Hembra", item.Sexo);
        Assert.Equal("Granja Norte", item.Granja);
        Assert.Equal("Activo", item.Estado);
    }

    [Fact]
    public async Task GetAnimalListAsync_ShouldExcludeSoftDeletedAnimals()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = new AnimalReportRepository(context);

        var items = await repository.GetAnimalListAsync(CreateFilter(keyword: "Eliminada"));

        Assert.Empty(items);
    }

    private static ReportAnimalListFilter CreateFilter(
        string? keyword = null,
        string? razaCode = null,
        string? colorCode = null,
        string? sexoCode = null,
        string? tipoAdquisicionCode = null,
        long? granjaId = null,
        string? estadoCode = null)
    {
        return new ReportAnimalListFilter(
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 31),
            keyword,
            razaCode,
            colorCode,
            sexoCode,
            tipoAdquisicionCode,
            granjaId,
            estadoCode);
    }

    private static TestGanaderiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestGanaderiaDbContext(options);
    }

    private static void Seed(GanaderiaDbContext context)
    {
        context.cat_razas.AddRange(
            new cat_raza { code = "HOL", nombre = "Holstein", activo = true },
            new cat_raza { code = "JER", nombre = "Jersey", activo = true });
        context.cat_sexos.AddRange(
            new cat_sexo { code = "H", nombre = "Hembra" },
            new cat_sexo { code = "M", nombre = "Macho" });
        context.cat_tipo_adquisicions.AddRange(
            new cat_tipo_adquisicion { code = "NAC", nombre = "Nacimiento", activo = true },
            new cat_tipo_adquisicion { code = "COMPRA", nombre = "Compra", activo = true });
        context.cat_colors.AddRange(
            new cat_color { code = "NEG", nombre = "Negro", activo = true },
            new cat_color { code = "CAF", nombre = "Cafe", activo = true });
        context.cat_estado_vacunos.AddRange(
            new cat_estado_vacuno { code = "ACT", nombre = "Activo" },
            new cat_estado_vacuno { code = "INACT", nombre = "Inactivo" });
        context.granjas.AddRange(
            new granja { id = 1, nombre = "Granja Norte", distrito_codigo = "010101", activo = true },
            new granja { id = 2, nombre = "Granja Sur", distrito_codigo = "010102", activo = true });
        context.v_vacuno_estado_vigentes.AddRange(
            new v_vacuno_estado_vigente { vacuno_id = 1, estado_code = "ACT", fecha_estado = new DateOnly(2023, 1, 1), fecha_registro = new DateTime(2023, 1, 1) },
            new v_vacuno_estado_vigente { vacuno_id = 2, estado_code = "INACT", fecha_estado = new DateOnly(2023, 1, 1), fecha_registro = new DateTime(2023, 1, 1) },
            new v_vacuno_estado_vigente { vacuno_id = 3, estado_code = "ACT", fecha_estado = new DateOnly(2023, 1, 1), fecha_registro = new DateTime(2023, 1, 1) });
        context.vacunos.AddRange(
            new vacuno
            {
                id = 1,
                codigo = "V001",
                nombre = "Lola",
                fecha_nacimiento = new DateOnly(2021, 5, 10),
                tipo_adquisicion_code = "NAC",
                raza_code = "HOL",
                color_code = "NEG",
                sexo_code = "H",
                granja_id = 1,
                fecha_registro = new DateOnly(2023, 1, 15),
                created_at = new DateTime(2023, 1, 15),
                updated_at = new DateTime(2023, 1, 15)
            },
            new vacuno
            {
                id = 2,
                codigo = "V002",
                nombre = "Toro",
                fecha_nacimiento = new DateOnly(2020, 3, 1),
                tipo_adquisicion_code = "COMPRA",
                raza_code = "JER",
                color_code = "CAF",
                sexo_code = "M",
                granja_id = 2,
                fecha_registro = new DateOnly(2023, 1, 20),
                created_at = new DateTime(2023, 1, 20),
                updated_at = new DateTime(2023, 1, 20)
            },
            new vacuno
            {
                id = 3,
                codigo = "V003",
                nombre = "Eliminada",
                fecha_nacimiento = new DateOnly(2022, 8, 1),
                tipo_adquisicion_code = "NAC",
                raza_code = "HOL",
                color_code = "NEG",
                sexo_code = "H",
                granja_id = 1,
                fecha_registro = new DateOnly(2023, 1, 25),
                created_at = new DateTime(2023, 1, 25),
                updated_at = new DateTime(2023, 1, 25),
                deleted_at = new DateTime(2023, 2, 1)
            });
        context.SaveChanges();
    }

    private sealed class TestGanaderiaDbContext : GanaderiaDbContext
    {
        public TestGanaderiaDbContext(DbContextOptions<GanaderiaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<v_vacuno_estado_vigente>().HasKey(entity => entity.vacuno_id);
        }
    }
}
