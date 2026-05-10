using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using ZooTech.Infrastructure.Persistence;
using ZooTech.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.InterfaceAdapters.IntegrationTests.Controllers;

public class AnimalsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnimalsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private void SeedDatabase(Guid tenantId, params AnimalEntity[] animals)
    {
        // Se utiliza el scope para obtener el DbContext factory y crear la BD del tenant
        using var scope = _factory.Services.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<ITenantDbContextFactory>();
        var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>() as TestTenantContext;
        tenantContext.TenantId = tenantId;

        using var db = contextFactory.CreateDbContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Animals.AddRange(animals);
        db.SaveChanges();
    }

    [Fact]
    public async Task Get_ListAnimals_WithTenantIsolation_ReturnsOnlyTenantData()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        SeedDatabase(tenantA, new AnimalEntity
        {
            Codigo = "TEN-A-01",
            Nombre = "Vaca A",
            Estado = "VIVO",
            RazaCode = "R1",
            RazaNombre = "Raza 1",
            ProcedenciaGranja = "G", ProcedenciaDistrito = "D", ProcedenciaProvincia = "P", ProcedenciaDepartamento = "Dep",
            FechaNacimiento = DateTime.UtcNow.AddDays(-5),
            FechaRegistro = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        });

        SeedDatabase(tenantB, new AnimalEntity
        {
            Codigo = "TEN-B-01",
            Nombre = "Vaca B",
            Estado = "VIVO",
            RazaCode = "R2",
            RazaNombre = "Raza 2",
            ProcedenciaGranja = "G", ProcedenciaDistrito = "D", ProcedenciaProvincia = "P", ProcedenciaDepartamento = "Dep",
            FechaNacimiento = DateTime.UtcNow.AddDays(-5),
            FechaRegistro = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        });

        var clientA = _factory.CreateClient();
        clientA.DefaultRequestHeaders.Add("X-Tenant-Id", tenantA.ToString());

        var clientB = _factory.CreateClient();
        clientB.DefaultRequestHeaders.Add("X-Tenant-Id", tenantB.ToString());

        // Act
        var responseA = await clientA.GetAsync("/v1/vacunos");
        var responseB = await clientB.GetAsync("/v1/vacunos");

        // Assert
        responseA.EnsureSuccessStatusCode();
        responseB.EnsureSuccessStatusCode();

        var jsonA = await responseA.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var jsonB = await responseB.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

        string codigoA = jsonA.GetProperty("data")[0].GetProperty("codigo").GetString();
        string codigoB = jsonB.GetProperty("data")[0].GetProperty("codigo").GetString();

        codigoA.Should().Be("TEN-A-01");
        codigoB.Should().Be("TEN-B-01");
    }

    [Fact]
    public async Task Get_ListAnimals_WithInvalidEstado_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "Tenant_Test");

        // Act
        var response = await client.GetAsync("/v1/vacunos?estado=INVALIDO");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        string errorCode = json.GetProperty("error").GetProperty("code").GetString();
        errorCode.Should().Be("VALIDATION_ERROR");
    }
}
