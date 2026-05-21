using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ZooTech.API.Models;
using ZooTech.API.Services;

namespace ZooTech.API.IntegrationTests;

public class VacunosApiTests
{
    [Fact]
    public async Task Login_WithDemoCredentials_ReturnsBearerToken()
    {
        await using var app = new WebApplicationFactory<Program>();
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
        await using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(DemoAuthService.DemoEmail, "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VacunoEndpoints_WithoutToken_ReturnUnauthorized()
    {
        await using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/vacunos");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListVacunos_WithToken_ReturnsSeededPagedData()
    {
        await using var app = new WebApplicationFactory<Program>();
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
        await using var app = new WebApplicationFactory<Program>();
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
    public async Task UpdateVacuno_ChangesDetailReturnedByApi()
    {
        await using var app = new WebApplicationFactory<Program>();
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
        await using var app = new WebApplicationFactory<Program>();
        using var client = CreateAuthorizedClient(app);

        var deleteResponse = await client.DeleteAsync("/api/vacunos/VAC_101");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/vacunos/VAC_101");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static HttpClient CreateAuthorizedClient(WebApplicationFactory<Program> app)
    {
        var client = app.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", DemoAuthService.DemoToken);
        return client;
    }
}
