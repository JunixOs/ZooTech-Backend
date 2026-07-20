using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public class FecundacionControllerTests : IClassFixture<ZooTechApiFactory>
{
    private readonly HttpClient _client;

    public FecundacionControllerTests(ZooTechApiFactory factory)
    {
        _client = factory.CreateTenantClient();
    }

    [Fact]
    public async Task ListarFecundaciones_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/fecundaciones");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<FecundacionItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarFecundacion_WhenRegistroExists_ShouldHideItFromList()
    {
        var created = await CreateFecundacionAsync();

        var deleteResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/fecundaciones/{created.Id}")
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Prueba de eliminacion de fecundacion."))
        });

        var errorBody = await deleteResponse.Content.ReadAsStringAsync();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, $"because {errorBody}");

        var getResponse = await _client.GetAsync($"/api/v1/fecundaciones/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EliminarFecundacion_WhenRegistroDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/v1/fecundaciones/999999")
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Registro inexistente."))
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<CreateFecundacionResponse> CreateFecundacionAsync()
    {
        var data = await GetFecundacionFormDataAsync();
        using var doc = JsonDocument.Parse(data.VacunoDonanteId.ToString());
        var request = new CreateFecundacionRequest(
            TipoFecundacion: data.TipoFecundacionCode,
            VacunoReceptorId: data.VacunoReceptorId,
            MachoODonante: doc.RootElement.Clone(),
            MachoExterno: false,
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.UtcNow),
            Responsable: "Responsable Test Integracion",
            Resultado: data.ResultadoCode,
            CodigoSemen: null,
            CodigoEmbrion: null,
            Observaciones: "Creado por prueba de integracion");

        var response = await _client.PostAsJsonAsync("/api/v1/fecundaciones", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<CreateFecundacionResponse>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        return content.Data!;
    }

    private async Task<FecundacionFormData> GetFecundacionFormDataAsync()
    {
        var options = await _client.GetFromJsonAsync<GeneralResponseDTO<FecundacionOptionsResponse>>(
            "/api/v1/fecundaciones/opciones");
        var hembras = await _client.GetFromJsonAsync<GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>>(
            "/api/v1/fecundaciones/vacunos?sexo=HEMBRA&soloDisponibles=true");
        var machos = await _client.GetFromJsonAsync<GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>>(
            "/api/v1/fecundaciones/vacunos?sexo=MACHO");

        options?.Data.Should().NotBeNull();
        var tipo = options!.Data!.Tipos
            .FirstOrDefault(item => item.Code.Equals("MONTA_NATURAL", StringComparison.OrdinalIgnoreCase))
            ?? options.Data.Tipos.First();
        var resultado = options.Data.Resultados.First();
        var receptor = hembras?.Data?.FirstOrDefault();
        var donante = machos?.Data?.FirstOrDefault();

        receptor.Should().NotBeNull("the tenant must contain an available female vacuno");
        donante.Should().NotBeNull("the tenant must contain an active male vacuno");

        return new FecundacionFormData(tipo.Code, resultado.Code, receptor!.Id, donante!.Id);
    }

    private sealed record FecundacionFormData(
        string TipoFecundacionCode,
        string ResultadoCode,
        long VacunoReceptorId,
        long VacunoDonanteId);
}
