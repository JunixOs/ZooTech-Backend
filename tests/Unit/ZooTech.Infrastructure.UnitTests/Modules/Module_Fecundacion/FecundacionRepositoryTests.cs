using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

namespace ZooTech.Infrastructure.UnitTests.Modules.Module_Fecundacion;

public sealed class FecundacionRepositoryTests
{
    [Fact]
    public async Task UpdateAsync_ShouldAssociateGeneratedId_WhenResponsibleIsNew()
    {
        await using var context = CreateContext();
        await SeedEditableFecundacionAsync(context);
        context.ChangeTracker.Clear();
        var repository = new FecundacionRepository(context);

        var result = await repository.UpdateAsync(
            1,
            CreateValidValues("Nuevo Responsable"));

        var responsible = await context.responsables
            .SingleAsync(item => item.nombre_completo == "Nuevo Responsable");
        var fecundacion = await context.fecundacions
            .Include(item => item.responsable)
            .SingleAsync(item => item.id == 1);

        Assert.NotNull(result);
        Assert.True(responsible.id > 0);
        Assert.Equal(responsible.id, fecundacion.responsable_id);
        Assert.Equal("Nuevo Responsable", fecundacion.responsable.nombre_completo);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReuseExistingResponsible()
    {
        await using var context = CreateContext();
        await SeedEditableFecundacionAsync(context);
        context.ChangeTracker.Clear();
        var repository = new FecundacionRepository(context);

        await repository.UpdateAsync(
            1,
            CreateValidValues("Responsable Actual"));

        Assert.Single(context.responsables);
        Assert.Equal(1, (await context.fecundacions.SingleAsync()).responsable_id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnValidationError_WhenStateDoesNotExist()
    {
        await using var context = CreateContext();
        context.cat_tipo_fecundacions.Add(new cat_tipo_fecundacion
        {
            code = "INSEMINACION_ARTIFICIAL",
            nombre = "Inseminacion artificial"
        });
        context.cat_resultado_fecundacions.Add(new cat_resultado_fecundacion
        {
            code = "PENDIENTE",
            nombre = "Pendiente"
        });
        context.fecundacions.Add(new fecundacion
        {
            id = 1,
            codigo = "FEC001",
            tipo_fecundacion_code = "INSEMINACION_ARTIFICIAL",
            vacuno_receptor_id = 1,
            fecha_procedimiento = new DateOnly(2026, 7, 20),
            responsable_id = 1,
            resultado_code = "PENDIENTE",
            created_at = new DateTime(2026, 7, 20),
            updated_at = new DateTime(2026, 7, 20)
        });
        await context.SaveChangesAsync();
        var repository = new FecundacionRepository(context);
        var values = new FecundacionUpdateValues(
            "INSEMINACION_ARTIFICIAL",
            1,
            "INTERNO",
            2,
            null,
            new DateOnly(2026, 7, 20),
            "Veterinario",
            "PENDIENTE",
            "ESTADO_INEXISTENTE",
            null,
            "SEM-001",
            null);

        var action = () => repository.UpdateAsync(1, values);

        await Assert.ThrowsAsync<FecundacionInvalidEstadoException>(action);
    }

    private static FecundacionUpdateValues CreateValidValues(string responsibleName)
        => new(
            "MONTA_NATURAL",
            1,
            "INTERNO",
            2,
            null,
            new DateOnly(2026, 7, 20),
            responsibleName,
            "PENDIENTE",
            "EN_ESPERA",
            null,
            null,
            null);

    private static async Task SeedEditableFecundacionAsync(GanaderiaDbContext context)
    {
        context.cat_sexos.AddRange(
            new cat_sexo { code = "H", nombre = "Hembra" },
            new cat_sexo { code = "M", nombre = "Macho" });
        context.cat_tipo_fecundacions.Add(new cat_tipo_fecundacion
        {
            code = "MONTA_NATURAL",
            nombre = "Monta natural"
        });
        context.cat_resultado_fecundacions.Add(new cat_resultado_fecundacion
        {
            code = "PENDIENTE",
            nombre = "Pendiente"
        });
        context.cat_estado_fecundacion_vacunos.Add(new cat_estado_fecundacion_vacuno
        {
            code = "EN_ESPERA",
            nombre = "En Espera"
        });
        context.cat_tipo_responsables.Add(new cat_tipo_responsable
        {
            code = "VET",
            nombre = "Veterinario"
        });
        context.responsables.Add(new responsable
        {
            id = 1,
            nombre_completo = "Responsable Actual",
            tipo_responsable_code = "VET",
            activo = true,
            created_at = new DateTime(2026, 7, 1)
        });
        context.vacunos.AddRange(
            CreateVacuno(1, "VAC001", "Luna", "H"),
            CreateVacuno(2, "VAC002", "Toro", "M"));
        context.fecundacions.Add(new fecundacion
        {
            id = 1,
            codigo = "FEC001",
            tipo_fecundacion_code = "MONTA_NATURAL",
            vacuno_receptor_id = 1,
            fecha_procedimiento = new DateOnly(2026, 7, 20),
            responsable_id = 1,
            resultado_code = "PENDIENTE",
            created_at = new DateTime(2026, 7, 20),
            updated_at = new DateTime(2026, 7, 20)
        });

        await context.SaveChangesAsync();
    }

    private static vacuno CreateVacuno(long id, string code, string name, string sexCode)
        => new()
        {
            id = id,
            codigo = code,
            nombre = name,
            fecha_nacimiento = new DateOnly(2022, 1, 1),
            tipo_adquisicion_code = "NAC",
            raza_code = "HOL",
            color_code = "NEG",
            sexo_code = sexCode,
            granja_id = 1,
            fecha_registro = new DateOnly(2026, 1, 1),
            created_at = new DateTime(2026, 1, 1),
            updated_at = new DateTime(2026, 1, 1)
        };

    private static GanaderiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GanaderiaDbContext(options);
    }
}
