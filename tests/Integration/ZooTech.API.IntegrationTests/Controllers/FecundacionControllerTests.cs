using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Support;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Models;
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
        var response = await _client.GetAsync(RequirementApiRoutes.Fecundaciones);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<FecundacionItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarFecundacion_WhenRegistroExists_ShouldHideItFromList()
    {
        var scenario = new FecundacionApiScenario(_client);
        var created = await scenario.CreateAsync();

        var deleteResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, RequirementApiRoutes.Fecundacion(created.Id))
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Prueba de eliminacion de fecundacion."))
        });

        var errorBody = await deleteResponse.Content.ReadAsStringAsync();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, $"because {errorBody}");

        var getResponse = await _client.GetAsync(RequirementApiRoutes.Fecundacion(created.Id));
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EliminarFecundacion_WhenRegistroDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            RequirementApiRoutes.Fecundacion(RequirementApiRoutes.MissingEntityId))
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest("Registro inexistente."))
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EliminarFecundacion_WhenReasonIsEmpty_ShouldReturnStructuredBadRequest()
    {
        var response = await _client.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            RequirementApiRoutes.Fecundacion(10))
        {
            Content = JsonContent.Create(new DeleteFecundacionRequest(" "))
        });
        var error = await response.Content.ReadFromJsonAsync<ErrorResponseModel>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.Error.FieldErrors.Should().ContainSingle(x =>
            x.Field == "razon" && x.Code == "FECUNDACION-DELETE-RAZON-REQUIRED");
    }
}
