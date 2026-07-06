using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public class VacunoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VacunoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptorsToRemove = services.Where(d => 
                    d.ServiceType == typeof(DbContextOptions<GanaderiaDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(GanaderiaDbContext) ||
                    d.ServiceType.Name.Contains("DbContextOptionsConfiguration")
                ).ToList();
                
                foreach (var d in descriptorsToRemove)
                {
                    services.Remove(d);
                }

                var dbName = $"ZooTechApiTestDb_{Guid.NewGuid()}";
                services.AddDbContext<GanaderiaDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GanaderiaDbContext>();
        SeedDatabase(db);
    }

    private void SeedDatabase(GanaderiaDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.cat_colors.Add(new cat_color { code = "NE", nombre = "Negro" });
        db.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = "NAC", nombre = "Nacimiento" });
        db.granjas.Add(new granja { id = 1, nombre = "Granja Test", distrito_codigo = "010101" });

        var vacuno = new vacuno
        {
            id = 1,
            codigo = "VAC-001",
            nombre = "Vaca Uno",
            raza_code = "HOL",
            sexo_code = "H",
            color_code = "NE",
            tipo_adquisicion_code = "NAC",
            fecha_nacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(-5)),
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            granja_id = 1
        };

        db.vacunos.Add(vacuno);
        db.SaveChanges();
    }

    [Fact]
    public async Task ListarVacunos_ReturnsOk_AndPagedResponse()
    {
        var response = await _client.GetAsync("/api/v1/vacunos?page=1&limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Page.Should().Be(1);
        content.Limit.Should().Be(5);
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetArbolGenealogico_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoExists_ReturnsExcelFile()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/1/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
