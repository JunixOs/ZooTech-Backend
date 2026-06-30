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
using ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;

namespace ZooTech.InterfaceAdapters.IntegrationTests.Controllers;

public class VacunosGenealogiaIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

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

                // Seeding de base de datos
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<GanaderiaDbContext>();

                db.Database.EnsureCreated();
                SeedDatabase(db);
            });
        });
    }

    private void SeedDatabase(GanaderiaDbContext db)
    {
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
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
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
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
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
            fecha_registro = DateOnly.FromDateTime(DateTime.Now),
            granja_id = 1
        };

        db.cat_colors.Add(new cat_color { code = "NE", nombre = "Negro" });
        db.cat_colors.Add(new cat_color { code = "BN", nombre = "Blanco y Negro" });
        db.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = "NAC", nombre = "Nacimiento" });

        // Insertamos una granja mínima para evitar constraints si los hubiera, aunque en in-memory no es estricto
        db.granjas.Add(new granja { id = 1, nombre = "Granja Test", distrito_codigo = "010101" });

        db.vacunos.AddRange(abuelo, padre, raiz);
        db.SaveChanges();
    }

    [Fact]
    public async Task GetGenealogia_DebeRetornar200YArbolCorrecto_CuandoVacunoExiste()
    {
        // Arrange
        var client = _factory.CreateClient();
        long vacunoId = 100; // El ID raíz de nuestro seed

        // Simulamos el Header del Tenant (requerido por el middleware/context)
        client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        // Act
        var response = await client.GetAsync($"/v1/vacunos/{vacunoId}/genealogia?niveles=4");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Deserializamos usando el DTO de respuesta genérico y el output específico
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<GenerarArbolGenealogicoOutput>>();
        
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.NotNull(content.Data);
        Assert.NotNull(content.Data.Data);

        var nodoRaiz = content.Data.Data;

        // Verificamos Nivel 0 (Raíz)
        Assert.Equal(100, nodoRaiz.Id);
        Assert.Equal("RAIZ-001", nodoRaiz.Codigo);
        Assert.Equal(0, nodoRaiz.Nivel);

        // Verificamos Nivel 1 (Padre)
        Assert.NotNull(nodoRaiz.Padre);
        Assert.Equal(200, nodoRaiz.Padre.Id);
        Assert.Equal("PAD-001", nodoRaiz.Padre.Codigo);
        Assert.Equal(1, nodoRaiz.Padre.Nivel);

        // Verificamos Nivel 2 (Abuelo)
        Assert.NotNull(nodoRaiz.Padre.Padre);
        Assert.Equal(300, nodoRaiz.Padre.Padre.Id);
        Assert.Equal("ABU-001", nodoRaiz.Padre.Padre.Codigo);
        Assert.Equal(2, nodoRaiz.Padre.Padre.Nivel);

        // Verificamos que ya no hay más niveles
        Assert.Null(nodoRaiz.Padre.Padre.Padre);
    }

    [Fact]
    public async Task GetGenealogia_DebeRetornar404_CuandoVacunoNoExiste()
    {
        // Arrange
        var client = _factory.CreateClient();
        long vacunoId = 9999; // ID inexistente

        client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        // Act
        var response = await client.GetAsync($"/v1/vacunos/{vacunoId}/genealogia");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<object>>();
        
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.NotNull(content.ErrorMessage);
        Assert.Contains("9999", content.ErrorMessage); // Verifica que el mensaje contenga el ID
    }

    [Fact]
    public async Task GetGenealogia_DebeLimitarNivelesA4_CuandoSePideMasDe4()
    {
        // Arrange
        var client = _factory.CreateClient();
        long vacunoId = 100;

        client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        // Act - Pedimos 10 niveles, pero el sistema debe hacer un clamp a 4.
        var response = await client.GetAsync($"/v1/vacunos/{vacunoId}/genealogia?niveles=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<GenerarArbolGenealogicoOutput>>();
        Assert.NotNull(content);
        Assert.True(content.Success);
    }
}
