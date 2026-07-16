using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;

namespace ZooTech.API.IntegrationTests.Controllers;

public class FecundacionControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public FecundacionControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task ListarFecundaciones_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/fecundaciones");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<List<FecundacionResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task EliminarFecundacion_WhenRegistroExists_ShouldHideItFromList()
    {
        var created = await CreateFecundacionAsync();

        var deleteResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/fecundaciones/{created.Id}")
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Prueba de eliminacion de fecundacion."))
        });

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/fecundaciones/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task EliminarFecundacion_WhenRegistroDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/v1/fecundaciones/999999")
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Registro inexistente."))
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<FecundacionCreatedResponse> CreateFecundacionAsync()
    {
        var data = await GetFecundacionFormDataAsync();
        using var doc = JsonDocument.Parse(data.VacunoDonanteId.ToString());
        var request = new CreateFecundacionRequest(
            TipoFecundacion: data.TipoFecundacionCode,
            VacunoReceptorId: data.VacunoReceptorId,
            MachoODonante: doc.RootElement.Clone(),
            MachoExterno: false,
            FechaProcedimiento: new DateOnly(2026, 1, 15),
            Responsable: "Responsable Test Integracion",
            Resultado: data.ResultadoCode,
            CodigoSemen: null,
            CodigoEmbrion: null,
            Observaciones: "Creado por prueba de integracion");

        var response = await _client.PostAsJsonAsync("/api/v1/fecundaciones", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<FecundacionCreatedResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        return content.Data!;
    }

    private async Task<FecundacionFormData> GetFecundacionFormDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GanaderiaDbContext>();

        var tipo = await db.cat_tipo_fecundacions
            .Select(x => x.code)
            .FirstAsync();
        var resultado = await db.cat_resultado_fecundacions
            .Select(x => x.code)
            .FirstAsync();
        var receptor = await db.vacunos
            .Where(x => x.deleted_at == null)
            .OrderBy(x => x.id)
            .Select(x => x.id)
            .FirstAsync();
        var donante = await db.vacunos
            .Where(x => x.deleted_at == null && x.id != receptor)
            .OrderBy(x => x.id)
            .Select(x => x.id)
            .FirstAsync();

        return new FecundacionFormData(tipo, resultado, receptor, donante);
    }

    private sealed record FecundacionCreatedResponse(long Id, string Codigo);

    private sealed record FecundacionFormData(
        string TipoFecundacionCode,
        string ResultadoCode,
        long VacunoReceptorId,
        long VacunoDonanteId);
}
