using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;

namespace ZooTech.Infrastructure.UnitTests.Reports;

public class FecundacionEstadoRepositoryTests
{
    [Fact]
    public async Task GetByVacunoIdAsync_WhenVacunoHasLatestFecundacion_ShouldReturnCurrentState()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = CreateRepository(context);

        var snapshot = await repository.GetByVacunoIdAsync(1);

        Assert.NotNull(snapshot);
        Assert.Equal(1, snapshot.VacunoId);
        Assert.Equal("V001", snapshot.CodigoVacuno);
        Assert.Equal(FecundacionEstadoConstants.EnProceso, snapshot.EstadoActual);
        Assert.False(snapshot.DisponibleNuevaFecundacion);
        Assert.Equal("F001", snapshot.CodigoFecundacion);
        Assert.Equal("Inseminacion artificial", snapshot.TipoFecundacion);
        Assert.Equal("Toro Bravo", snapshot.ToroDonante);
        Assert.Equal("Dra. Campos", snapshot.Responsable);
    }

    [Fact]
    public async Task GetByVacunoIdAsync_WhenVacunoHasNoFecundacion_ShouldReturnSinEstado()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = CreateRepository(context);

        var snapshot = await repository.GetByVacunoIdAsync(2);

        Assert.NotNull(snapshot);
        Assert.Equal(FecundacionEstadoConstants.SinEstado, snapshot.EstadoActual);
        Assert.True(snapshot.DisponibleNuevaFecundacion);
        Assert.Null(snapshot.CodigoFecundacion);
    }

    [Fact]
    public async Task UpdateEstadoAsync_ShouldAddHistoryAndUpdateFecundacionAudit()
    {
        await using var context = CreateContext();
        Seed(context);
        var repository = CreateRepository(context);

        await repository.UpdateEstadoAsync(10, "CONF", 7);

        var fecundacion = await context.fecundacions.SingleAsync(item => item.id == 10);
        var latest = await context.vacuno_estado_fecundacion_historials
            .OrderByDescending(item => item.id)
            .FirstAsync(item => item.fecundacion_id == 10);

        Assert.Equal(7, fecundacion.updated_by);
        Assert.Equal(new DateTime(2026, 1, 2, 8, 30, 0), fecundacion.updated_at);
        Assert.Equal("CONF", latest.estado_fecundacion_code);
        Assert.Equal(7, latest.created_by);
        Assert.Equal(new DateOnly(2026, 1, 2), latest.fecha_actualizacion);
        Assert.Single(context.bitacora_auditoria);
    }

    private static FecundacionEstadoRepository CreateRepository(GanaderiaDbContext context)
    {
        return new FecundacionEstadoRepository(context, new FakeDateTimeProvider());
    }

    private static GanaderiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GanaderiaDbContext(options);
    }

    private static void Seed(GanaderiaDbContext context)
    {
        context.cat_modulos.Add(new cat_modulo
        {
            code = "REPRODUCCION",
            nombre = "Reproduccion",
            activo = true
        });
        context.cat_sexos.AddRange(
            new cat_sexo { code = "H", nombre = "Hembra" },
            new cat_sexo { code = "M", nombre = "Macho" });
        context.cat_tipo_fecundacions.Add(new cat_tipo_fecundacion
        {
            code = "IA",
            nombre = "Inseminacion artificial"
        });
        context.cat_resultado_fecundacions.Add(new cat_resultado_fecundacion
        {
            code = "PEND",
            nombre = "Pendiente"
        });
        context.cat_estado_fecundacion_vacunos.AddRange(
            new cat_estado_fecundacion_vacuno { code = "PEND", nombre = FecundacionEstadoConstants.Pendiente },
            new cat_estado_fecundacion_vacuno { code = "PROC", nombre = FecundacionEstadoConstants.EnProceso },
            new cat_estado_fecundacion_vacuno { code = "CONF", nombre = FecundacionEstadoConstants.Confirmada });
        context.responsables.Add(new responsable
        {
            id = 3,
            nombre_completo = "Dra. Campos",
            tipo_responsable_code = "VET",
            activo = true,
            created_at = new DateTime(2026, 1, 1)
        });
        context.vacunos.AddRange(
            new vacuno
            {
                id = 1,
                codigo = "V001",
                nombre = "Lola",
                fecha_nacimiento = new DateOnly(2022, 1, 1),
                tipo_adquisicion_code = "NAC",
                raza_code = "HOL",
                color_code = "NEG",
                sexo_code = "H",
                granja_id = 1,
                fecha_registro = new DateOnly(2025, 1, 1),
                created_at = new DateTime(2025, 1, 1),
                updated_at = new DateTime(2025, 1, 1)
            },
            new vacuno
            {
                id = 2,
                codigo = "V002",
                nombre = "Nina",
                fecha_nacimiento = new DateOnly(2022, 2, 1),
                tipo_adquisicion_code = "NAC",
                raza_code = "HOL",
                color_code = "NEG",
                sexo_code = "H",
                granja_id = 1,
                fecha_registro = new DateOnly(2025, 1, 1),
                created_at = new DateTime(2025, 1, 1),
                updated_at = new DateTime(2025, 1, 1)
            });
        context.reproductor_externos.Add(new reproductor_externo
        {
            id = 5,
            nombre = "Toro Bravo",
            activo = true,
            created_at = new DateTime(2025, 1, 1)
        });
        context.fecundacions.Add(new fecundacion
        {
            id = 10,
            codigo = "F001",
            tipo_fecundacion_code = "IA",
            vacuno_receptor_id = 1,
            fecha_procedimiento = new DateOnly(2026, 1, 1),
            responsable_id = 3,
            resultado_code = "PEND",
            observaciones_veterinarias = "Sin observaciones",
            created_at = new DateTime(2026, 1, 1),
            updated_at = new DateTime(2026, 1, 1)
        });
        context.fecundacion_donantes.Add(new fecundacion_donante
        {
            fecundacion_id = 10,
            tipo_donante = "EXT",
            externo_donante_id = 5
        });
        context.vacuno_estado_fecundacion_historials.AddRange(
            new vacuno_estado_fecundacion_historial
            {
                id = 1,
                vacuno_id = 1,
                fecundacion_id = 10,
                estado_fecundacion_code = "PEND",
                fecha_actualizacion = new DateOnly(2026, 1, 1),
                created_at = new DateTime(2026, 1, 1, 7, 0, 0)
            },
            new vacuno_estado_fecundacion_historial
            {
                id = 2,
                vacuno_id = 1,
                fecundacion_id = 10,
                estado_fecundacion_code = "PROC",
                fecha_actualizacion = new DateOnly(2026, 1, 1),
                created_at = new DateTime(2026, 1, 1, 9, 0, 0)
            });
        context.SaveChanges();
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime ServerNow => new(2026, 1, 2, 8, 30, 0);

        public DateTime UtcNow => new(2026, 1, 2, 13, 30, 0, DateTimeKind.Utc);
    }
}
