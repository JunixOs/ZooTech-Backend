using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.DTOs.Responses;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.IntegrationTests.Controllers;

public class VacunosGenealogiaIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VacunosGenealogiaIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Configuramos el WebApplicationFactory para usar InMemoryDatabase
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover el DbContext original y todas las opciones
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

                // Inyectar base de datos en memoria para las pruebas
                var dbName = $"ZooTechIntegrationDb_{Guid.NewGuid()}";
                services.AddDbContext<GanaderiaDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });

        // Inicializamos el cliente HTTP para disparar la inicialización del TestServer
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        // Seeding de base de datos usando factory services
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GanaderiaDbContext>();
        SeedDatabase(db);
    }

    private void SeedDatabase(GanaderiaDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Insertamos registros de la tabla 'cat_raza' si es necesario (el repositorio InMemory no hace LEFT JOIN, 
        // pero mapea raza_code si no está la navegación, así que no es estrictamente necesario, pero lo agregamos por completitud)
        db.cat_razas.Add(new cat_raza { code = "ANG", nombre = "Angus" });
        db.cat_razas.Add(new cat_raza { code = "HOL", nombre = "Holstein" });

        // Nivel 2: Abuelo
        var abuelo = new vacuno 
        { 
            id = 300, 
            codigo = "ABU-001", 
            nombre = "Abuelo Toro", 
            raza_code = "ANG", 
            sexo_code = "M", 
            color_code = "NE",
            tipo_adquisicion_code = "NAC",
            fecha_nacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(-10)),
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            granja_id = 1
        };

        // Nivel 1: Padre
        var padre = new vacuno 
        { 
            id = 200, 
            codigo = "PAD-001", 
            nombre = "Padre Toro", 
            raza_code = "ANG", 
            sexo_code = "M", 
            color_code = "NE",
            tipo_adquisicion_code = "NAC",
            padre_id = 300, // Hijo del abuelo
            fecha_nacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(-5)),
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            granja_id = 1
        };

        // Nivel 0: Raíz
        var raiz = new vacuno 
        { 
            id = 100, 
            codigo = "RAIZ-001", 
            nombre = "Hija Vaca", 
            raza_code = "HOL", 
            sexo_code = "H", 
            color_code = "BN",
            tipo_adquisicion_code = "NAC",
            padre_id = 200, // Hija del padre
            fecha_nacimiento = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            granja_id = 1
        };

        db.cat_colors.Add(new cat_color { code = "NE", nombre = "Negro" });
        db.cat_colors.Add(new cat_color { code = "BN", nombre = "Blanco y Negro" });
        db.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = "NAC", nombre = "Nacimiento" });

        // Seed geo data to satisfy non-nullable navigation joins
        db.geo_departamentos.Add(new geo_departamento { codigo = "01", nombre = "Lima" });
        db.geo_provincia.Add(new geo_provincium { codigo = "0101", nombre = "Lima", departamento_codigo = "01" });
        db.geo_distritos.Add(new geo_distrito { codigo = "010101", nombre = "Lima", provincia_codigo = "0101" });

        // Insertamos una granja mínima para evitar constraints si los hubiera, aunque en in-memory no es estricto
        db.granjas.Add(new granja { id = 1, nombre = "Granja Test", distrito_codigo = "010101" });

        db.vacunos.AddRange(abuelo, padre, raiz);
        db.SaveChanges();
    }

    [Fact]
    public async Task GetGenealogia_DebeRetornar200YArbolCorrecto_CuandoVacunoExiste()
    {
        // Arrange
        long vacunoId = 100; // El ID raíz de nuestro seed

        // Act
        var response = await _client.GetAsync($"/api/v1/vacunos/{vacunoId}/genealogia?niveles=4");

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(HttpStatusCode.OK == response.StatusCode, $"Response: {body}");

        // Deserializamos usando el DTO de respuesta genérico y la lista plana de GetArbolGenealogicoItem
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<List<GetArbolGenealogicoItem>>>();
        
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.NotNull(content.Data);

        var arbol = content.Data;
        Assert.Equal(3, arbol.Count);

        var nodoRaiz = arbol.FirstOrDefault(n => n.Id == 100);
        Assert.NotNull(nodoRaiz);
        Assert.Equal("RAIZ-001", nodoRaiz.Codigo);
        Assert.Equal(1, nodoRaiz.Nivel);

        var nodoPadre = arbol.FirstOrDefault(n => n.Id == 200);
        Assert.NotNull(nodoPadre);
        Assert.Equal("PAD-001", nodoPadre.Codigo);
        Assert.Equal(2, nodoPadre.Nivel);

        var nodoAbuelo = arbol.FirstOrDefault(n => n.Id == 300);
        Assert.NotNull(nodoAbuelo);
        Assert.Equal("ABU-001", nodoAbuelo.Codigo);
        Assert.Equal(3, nodoAbuelo.Nivel);
    }

    [Fact]
    public async Task GetGenealogia_DebeRetornar404_CuandoVacunoNoExiste()
    {
        // Arrange
        long vacunoId = 9999; // ID inexistente

        // Act
        var response = await _client.GetAsync($"/api/v1/vacunos/{vacunoId}/genealogia");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        
        Assert.NotNull(content);
        Assert.Equal("VACUNO_NOT_FOUND", content.Error.Code);
        Assert.Contains("9999", content.Error.Message); // Verifica que el mensaje contenga el ID
    }

    [Fact]
    public async Task GetGenealogia_DebeLimitarNivelesA4_CuandoSePideMasDe4()
    {
        // Arrange
        long vacunoId = 100;

        // Act - Pedimos 10 niveles, pero el sistema debe hacer un clamp a 4.
        var response = await _client.GetAsync($"/api/v1/vacunos/{vacunoId}/genealogia?niveles=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<List<GetArbolGenealogicoItem>>>();
        Assert.NotNull(content);
        Assert.True(content.Success);
    }
}
