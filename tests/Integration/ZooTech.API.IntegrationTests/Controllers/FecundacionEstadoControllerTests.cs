using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.API.IntegrationTests.Support;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public sealed class FecundacionEstadoControllerTests : IClassFixture<ZooTechApiFactory>
{
    private readonly ZooTechApiFactory _factory;

    public FecundacionEstadoControllerTests(ZooTechApiFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("zootecniaunas.zentrycorp.local")]
    [InlineData("elroble.zentrycorp.local")]
    public async Task GetEstado_DebeResolverLaBaseDelTenantEnRutaNuevaYAlias(string tenantHost)
    {
        using var client = _factory.CreateTenantClient(tenantHost);
        var vacuno = await GetHembraAsync(client);

        var current = await client.GetAsync(
            $"/api/v1/reproduccion/fecundacion/estado/{vacuno.Id}");
        var legacy = await client.GetAsync(
            $"/api/reproduccion/fecundacion/estado/{vacuno.Id}");

        current.StatusCode.Should().Be(HttpStatusCode.OK);
        legacy.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateEstado_DebeUsarUsuarioAutenticadoSinHeaderDeUsuario()
    {
        using var client = _factory.CreateTenantClient();
        var scenario = new FecundacionApiScenario(client);
        var created = await scenario.CreateAsync();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/reproduccion/fecundacion/estado/{created.Id}",
            new UpdateFecundacionEstadoRequest("GESTANTE"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content
            .ReadFromJsonAsync<GeneralResponseDTO<FecundacionEstadoResponse>>();
        content?.Data?.EstadoActual.Should().Be("Gestante");
    }

    private static async Task<VacunoItemResponse> GetHembraAsync(HttpClient client)
    {
        var response = await client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            RequirementApiRoutes.VacunosPage(1, 5, VacunosBasicSeeder.HembraCode));
        return response!.Data!.Single(item => item.Codigo == VacunosBasicSeeder.HembraCode);
    }
}
