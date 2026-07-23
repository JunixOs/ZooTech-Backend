using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Tenant;

public sealed class FecundacionCatalogSeederTests
{
    [Fact]
    public async Task SeedAsync_DebeCompletarCatalogoVacioSinDuplicarEnRepeticion()
    {
        var (sut, options) = CreateSut();

        var first = await sut.SeedAsync("tenant-a");
        var second = await sut.SeedAsync("tenant-a");

        first.InsertedCount.Should().Be(3);
        second.InsertedCount.Should().Be(0);
        await using var verification = new GanaderiaDbContext(options);
        (await verification.cat_estado_fecundacion_vacunos.CountAsync()).Should().Be(3);
    }

    [Fact]
    public async Task SeedAsync_DebeInsertarSoloEstadosAusentes()
    {
        var (sut, options) = CreateSut();
        await using (var context = new GanaderiaDbContext(options))
        {
            context.cat_estado_fecundacion_vacunos.Add(
                new cat_estado_fecundacion_vacuno
                {
                    code = "GESTANTE",
                    nombre = "Nombre conservado",
                    descripcion = "Descripción conservada"
                });
            await context.SaveChangesAsync();
        }

        var result = await sut.SeedAsync("tenant-a");

        result.InsertedCount.Should().Be(2);
        await using var verification = new GanaderiaDbContext(options);
        var states = await verification.cat_estado_fecundacion_vacunos
            .OrderBy(state => state.code)
            .ToListAsync();
        states.Should().HaveCount(3);
        states.Single(state => state.code == "GESTANTE").nombre.Should().Be("Nombre conservado");
    }

    private static (FecundacionCatalogSeeder Sut, DbContextOptions<GanaderiaDbContext> Options) CreateSut()
    {
        var root = new InMemoryDatabaseRoot();
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase("tenant-a", root)
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        var factory = new Mock<IGanaderiaDbContextFactory>();
        factory
            .Setup(item => item.CreateDbContextBySpecificDatabaseName(
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Returns(() => new GanaderiaDbContext(options));

        return (new FecundacionCatalogSeeder(factory.Object), options);
    }
}
