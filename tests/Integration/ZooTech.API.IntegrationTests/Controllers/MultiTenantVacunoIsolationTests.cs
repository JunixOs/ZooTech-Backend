using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.API.IntegrationTests.Support;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public sealed class MultiTenantVacunoIsolationTests : IClassFixture<ZooTechApiFactory>
{
    private readonly ZooTechApiFactory _factory;

    public MultiTenantVacunoIsolationTests(ZooTechApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListadoRegistroYEstado_DebenResolverDatosAisladosConIdsDiferentes()
    {
        const string firstSubdomain = "zootecniaunas";
        const string secondSubdomain = "elroble";
        using var firstClient = _factory.CreateTenantClient($"{firstSubdomain}.zentrycorp.local");
        using var secondClient = _factory.CreateTenantClient($"{secondSubdomain}.zentrycorp.local");

        var first = await FindMarkerAsync(firstClient, firstSubdomain);
        var second = await FindMarkerAsync(secondClient, secondSubdomain);
        first.Id.Should().NotBe(second.Id);

        var firstReport = await firstClient.GetAsync(
            $"/api/v1/vacunos/{first.Id}/reporte?formato=json");
        var secondReport = await secondClient.GetAsync(
            $"/api/v1/vacunos/{second.Id}/reporte?formato=json");
        firstReport.StatusCode.Should().Be(HttpStatusCode.OK);
        secondReport.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstState = await firstClient.GetAsync(
            $"/api/v1/reproduccion/fecundacion/estado/{first.Id}");
        var secondState = await secondClient.GetAsync(
            $"/api/v1/reproduccion/fecundacion/estado/{second.Id}");
        firstState.StatusCode.Should().Be(HttpStatusCode.OK);
        secondState.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<VacunoItemResponse> FindMarkerAsync(
        HttpClient client,
        string subdomain)
    {
        var code = TenantIsolationSeeder.GetCode(subdomain);
        var response = await client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            RequirementApiRoutes.VacunosPage(1, 5, code));
        return response!.Data!.Single(item => item.Codigo == code);
    }
}
