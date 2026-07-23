using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Models;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

namespace ZooTech.Infrastructure.UnitTests.Modules.Module_Vacuno;

public sealed class VacunoRepositoryPhotoPersistenceTests
{
    [Fact]
    public async Task AddAsync_WhenPhotoCatalogsAreMissing_CreatesTenantCatalogsAndAssociation()
    {
        await using var context = CreateContext();
        var repository = new VacunoRepository(context);
        var vacuno = CreateVacuno();
        var photo = CreatePhoto("PNG", "image/png");

        await repository.AddAsync(
            vacuno,
            precioCompra: null,
            aptoPara: null,
            fechaUtilizacion: null,
            foto: photo);
        await context.SaveChangesAsync();

        context.cat_tipo_archivos.Should().ContainSingle(item =>
            item.extension == ".png" && item.mime_type == "image/png");
        context.cat_modulos.Should().ContainSingle(item =>
            item.code == "VACUNO" && item.activo);
        context.vacuno_fotos.Should().ContainSingle(item =>
            item.es_principal && item.archivo.extension == ".png");
    }

    [Fact]
    public async Task AddAsync_WhenPluralModuleAndJpegCatalogExist_ReusesConfiguredCodes()
    {
        await using var context = CreateContext();
        context.cat_modulos.Add(new cat_modulo
        {
            code = "VACUNOS",
            nombre = "Modulo de Vacunos",
            activo = true
        });
        context.cat_tipo_archivos.Add(new cat_tipo_archivo
        {
            extension = ".jpg",
            mime_type = "image/jpeg"
        });
        await context.SaveChangesAsync();

        var repository = new VacunoRepository(context);
        await repository.AddAsync(
            CreateVacuno(),
            precioCompra: null,
            aptoPara: null,
            fechaUtilizacion: null,
            foto: CreatePhoto("JPG", "image/jpeg"));
        await context.SaveChangesAsync();

        context.cat_modulos.Should().ContainSingle();
        context.cat_tipo_archivos.Should().ContainSingle();
        context.archivos.Should().ContainSingle(item =>
            item.modulo_code == "VACUNOS" && item.extension == ".jpg");
    }

    private static GanaderiaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new GanaderiaDbContext(options);
    }

    private static Vacuno CreateVacuno()
        => Vacuno.Rehydrate(
            id: 1,
            codigo: "VAC9001",
            nombre: "Foto Test",
            fechaNacimiento: new DateOnly(2026, 7, 20),
            tipoAdquisicionCode: "DONACION",
            razaCode: "CRIOLLO",
            colorCode: "NEGRO",
            sexoCode: "MACHO",
            padreId: null,
            madreId: null,
            granjaId: 1,
            observaciones: null,
            fechaRegistro: new DateOnly(2026, 7, 23),
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 1,
            updatedBy: null,
            deletedBy: null);

    private static VacunoPhotoMetadata CreatePhoto(string extension, string contentType)
        => new(
            OriginalName: $"foto.{extension.ToLowerInvariant()}",
            StoredName: $"stored.{extension.ToLowerInvariant()}",
            RelativePath: $"1/vacunos/stored.{extension.ToLowerInvariant()}",
            Extension: extension,
            ContentType: contentType,
            SizeBytes: 128,
            Sha256: new string('a', 64));
}
