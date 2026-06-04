using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ZooTech.API.Models;
using ZooTech.API.Services;

namespace ZooTech.API.IntegrationTests;

public class VacunosApiTests
{
    [Fact]
    public async Task Login_WithDemoCredentials_ReturnsBearerToken()
    {
        await using var app = CreateInMemoryFactory();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(DemoAuthService.DemoEmail, DemoAuthService.DemoPassword));

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(payload);
        Assert.Equal("Bearer", payload.TokenType);
        Assert.Equal(DemoAuthService.DemoToken, payload.Token);
        Assert.Equal(DemoAuthService.DemoEmail, payload.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        await using var app = CreateInMemoryFactory();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(DemoAuthService.DemoEmail, "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VacunoEndpoints_WithoutToken_ReturnUnauthorized()
    {
        await using var app = CreateInMemoryFactory();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/vacunos");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListVacunos_WithToken_ReturnsSeededPagedData()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var response = await client.GetFromJsonAsync<PagedResponse<VacunoResponse>>(
            "/api/vacunos?page=1&pageSize=10");

        Assert.NotNull(response);
        Assert.Equal(1, response.Page);
        // At least one item must be returned by the API
        Assert.True(response.Items.Count > 0, "Esperado al menos 1 vacuno en la respuesta");
        // TotalItems must be consistent with returned items
        Assert.True(response.TotalItems >= response.Items.Count);
        // Each returned item should contain minimal fields
        Assert.All(response.Items, v =>
        {
            Assert.False(string.IsNullOrWhiteSpace(v.Codigo));
            Assert.False(string.IsNullOrWhiteSpace(v.Nombre));
        });
    }

    [Fact]
    public async Task GetVacunoByCodigo_ReturnsFullDetail()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        // Obtain a real vacuno code from the list endpoint to avoid hard-coded assumptions
        var list = await client.GetFromJsonAsync<PagedResponse<VacunoResponse>>("/api/vacunos?page=1&pageSize=10");
        Assert.NotNull(list);
        Assert.True(list.Items.Count > 0, "No se encontraron vacunos para comprobar el detalle");

        var sample = list.Items[0];
        var vacuno = await client.GetFromJsonAsync<VacunoResponse>($"/api/vacunos/{sample.Codigo}");

        Assert.NotNull(vacuno);
        // Verify that the details returned match the list entry
        Assert.Equal(sample.Codigo, vacuno.Codigo);
        Assert.Equal(sample.Nombre, vacuno.Nombre);
    }

    [Fact]
    public async Task ActivityStats_WithDateRange_ReturnsDailySeriesAndSummary()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var response = await client.GetFromJsonAsync<VacunoActividadStatsResponse>(
            "/api/vacunos/estadisticas/actividad?fechaInicio=2023-01-01&fechaFin=2023-01-05");

        Assert.NotNull(response);
        Assert.Equal(new DateOnly(2023, 1, 1), response.FechaInicio);
        Assert.Equal(new DateOnly(2023, 1, 5), response.FechaFin);
        Assert.Equal(5, response.Points.Count);
        Assert.True(response.Mayor >= response.Menor);
        Assert.All(response.Points, point => Assert.True(point.Cantidad >= 0));
    }

    [Fact]
    public async Task ActivityStats_WithInvalidDateRange_ReturnsBadRequest()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var response = await client.GetAsync(
            "/api/vacunos/estadisticas/actividad?fechaInicio=2023-01-05&fechaFin=2023-01-01");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Genealogia_WithExistingVacuno_ReturnsTreeUpToRequestedLevels()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var arbol = await client.GetFromJsonAsync<VacunoGenealogiaResponse>(
            "/api/vacunos/VAC_101/genealogia?niveles=4");

        Assert.NotNull(arbol);
        Assert.Equal("VAC_101", arbol.Codigo);
        Assert.Equal(0, arbol.Nivel);
        Assert.False(string.IsNullOrWhiteSpace(arbol.Procedencia));
        Assert.NotNull(arbol.Padre);
        Assert.NotNull(arbol.Madre);
        Assert.Equal("VAC_0021", arbol.Padre.Codigo);
        Assert.Equal("VAC_0147", arbol.Madre.Codigo);
        Assert.Equal(1, arbol.Padre.Nivel);
        Assert.NotNull(arbol.Padre.Padre);
        Assert.NotNull(arbol.Padre.Madre);
        Assert.Equal("VAC_0001", arbol.Padre.Padre.Codigo);
        Assert.Equal("VAC_0002", arbol.Padre.Madre.Codigo);
        Assert.Equal(2, arbol.Padre.Padre.Nivel);
    }

    [Fact]
    public async Task Genealogia_WithUnknownVacuno_ReturnsNotFound()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var response = await client.GetAsync("/api/vacunos/VAC_NO_EXISTE/genealogia");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorEnvelope>();
        Assert.NotNull(error);
        Assert.Equal("VACUNO_NOT_FOUND", error.Error.Code);
    }

    [Fact]
    public async Task Genealogia_WithOneLevel_DoesNotReturnGrandparents()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var arbol = await client.GetFromJsonAsync<VacunoGenealogiaResponse>(
            "/api/vacunos/VAC_101/genealogia?niveles=1");

        Assert.NotNull(arbol);
        Assert.NotNull(arbol.Padre);
        Assert.NotNull(arbol.Madre);
        Assert.Null(arbol.Padre.Padre);
        Assert.Null(arbol.Padre.Madre);
        Assert.Null(arbol.Madre.Padre);
        Assert.Null(arbol.Madre.Madre);
    }

    [Fact]
    public async Task RegisterVacuno_WithValidForm_CreatesVacuno()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        using var form = new MultipartFormDataContent
        {
            { new StringContent("VACREG01"), "codigo" },
            { new StringContent("Aurora"), "nombre" },
            { new StringContent("2024-01-10"), "fechaNacimiento" },
            { new StringContent("monta"), "adquisicionPor" },
            { new StringContent("Jersey"), "raza" },
            { new StringContent("Marron"), "color" },
            { new StringContent("hembra"), "sexo" },
            { new StringContent("VACPADRE1"), "codigoPadre" },
            { new StringContent("VACMADRE1"), "codigoMadre" },
            { new StringContent("Las Palmas"), "granja" },
            { new StringContent("Tulumayo"), "distrito" },
            { new StringContent("Huanuco"), "departamento" },
            { new StringContent("Leoncio Prado"), "provincia" },
            { new StringContent("produccion_leche"), "aptoPara" },
            { new StringContent("2024-01-15"), "fechaEspecificacion" },
            { new StringContent("Registro creado desde prueba."), "observaciones" }
        };

        var response = await client.PostAsync("/api/vacunos", form);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<RegistrarVacunoResponse>();
        Assert.NotNull(created);
        Assert.Equal("VACREG01", created.Codigo);
        Assert.Equal("Aurora", created.Nombre);

        var detail = await client.GetFromJsonAsync<VacunoResponse>("/api/vacunos/VACREG01");
        Assert.NotNull(detail);
        Assert.Equal("VACREG01", detail.Codigo);
        Assert.Equal("Aurora", detail.Nombre);
    }

    [Fact]
    public async Task UpdateVacuno_ChangesDetailReturnedByApi()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var current = await client.GetFromJsonAsync<VacunoResponse>("/api/vacunos/VAC_101");
        Assert.NotNull(current);

        var request = new UpdateVacunoRequest(
            Nombre: "Duquesa Editada",
            FechaNacimiento: current.FechaNacimiento,
            AdquisicionPor: current.AdquisicionPor,
            Raza: current.Raza,
            Color: current.Color,
            Sexo: current.Sexo,
            CodigoPadre: current.CodigoPadre,
            CodigoMadre: current.CodigoMadre,
            Granja: current.Granja,
            Distrito: current.Distrito,
            Departamento: current.Departamento,
            Provincia: current.Provincia,
            AptoPara: current.AptoPara,
            FechaRegistroFuncion: current.FechaRegistroFuncion,
            Observaciones: "Cambio validado desde prueba de integracion.",
            FotoUrl: current.FotoUrl,
            Estado: current.Estado);

        var updateResponse = await client.PutAsJsonAsync("/api/vacunos/VAC_101", request);
        updateResponse.EnsureSuccessStatusCode();

        var updated = await client.GetFromJsonAsync<VacunoResponse>("/api/vacunos/VAC_101");

        Assert.NotNull(updated);
        Assert.Equal("Duquesa Editada", updated.Nombre);
        Assert.Equal("Cambio validado desde prueba de integracion.", updated.Observaciones);
    }

    [Fact]
    public async Task DeleteVacuno_RemovesItFromReadModel()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var deleteResponse = await client.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            "/api/vacunos/VAC_101")
        {
            Content = JsonContent.Create(new DeleteVacunoRequest("Registro duplicado de prueba."))
        });

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deleted = await deleteResponse.Content.ReadFromJsonAsync<DeleteVacunoResponse>();
        Assert.NotNull(deleted);
        Assert.Equal("VAC_101", deleted.Codigo);
        Assert.Equal("Registro duplicado de prueba.", deleted.MotivoEliminacion);

        var getResponse = await client.GetAsync("/api/vacunos/VAC_101");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteVacuno_WithoutReason_ReturnsBadRequest()
    {
        await using var app = CreateInMemoryFactory();
        using var client = CreateAuthorizedClient(app);

        var deleteResponse = await client.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            "/api/vacunos/VAC_101")
        {
            Content = JsonContent.Create(new DeleteVacunoRequest(" "))
        });

        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
    }

    private static HttpClient CreateAuthorizedClient(WebApplicationFactory<Program> app)
    {
        var client = app.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", DemoAuthService.DemoToken);
        return client;
    }

    private static WebApplicationFactory<Program> CreateInMemoryFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Default"] = string.Empty
                    });
                });
            });
    }
}
