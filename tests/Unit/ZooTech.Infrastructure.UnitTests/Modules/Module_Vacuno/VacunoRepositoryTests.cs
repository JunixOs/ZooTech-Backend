using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

namespace ZooTech.Infrastructure.UnitTests.Modules.Module_Vacuno;

public class VacunoRepositoryTests
{
    [Fact]
    public async Task ListReferencesAsync_ExcludesVacunosWhoseLatestStateIsMuerto()
    {
        await using var context = CreateContext();
        context.vacunos.AddRange(
            CreateVacuno(1, "VIVO"),
            CreateVacuno(2, "MUERTO"));
        context.vacuno_estado_historials.AddRange(
            CreateState(1, 1, "SANO", new DateOnly(2026, 1, 1)),
            CreateState(2, 2, "SANO", new DateOnly(2026, 1, 1)),
            CreateState(3, 2, "MUERTO", new DateOnly(2026, 1, 2)));
        await context.SaveChangesAsync();

        var repository = new VacunoRepository(context);

        var references = await repository.ListReferencesAsync();

        Assert.Single(references);
        Assert.Equal("VIVO", references[0].Codigo);
        Assert.Equal("SANO", references[0].EstadoCode);
    }

    private static GanaderiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GanaderiaDbContext(options);
    }

    private static vacuno CreateVacuno(long id, string codigo) => new()
    {
        id = id,
        codigo = codigo,
        nombre = codigo,
        fecha_nacimiento = new DateOnly(2020, 1, 1),
        tipo_adquisicion_code = "NACIMIENTO",
        raza_code = "HOLSTEIN",
        color_code = "NEGRO",
        sexo_code = "HEMBRA",
        granja_id = 1,
        fecha_registro = new DateOnly(2026, 1, 1),
        created_at = new DateTime(2026, 1, 1),
        updated_at = new DateTime(2026, 1, 1),
    };

    private static vacuno_estado_historial CreateState(
        long id,
        long vacunoId,
        string estadoCode,
        DateOnly fechaEstado) => new()
    {
        id = id,
        vacuno_id = vacunoId,
        estado_code = estadoCode,
        fecha_estado = fechaEstado,
        created_at = fechaEstado.ToDateTime(TimeOnly.MinValue),
    };
}
