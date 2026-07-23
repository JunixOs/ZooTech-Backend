using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Support;

internal sealed class VacunoApiScenario : IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly List<long> _createdIds = [];

    public VacunoApiScenario(HttpClient client) => _client = client;

    public async Task<CreateVacunoRequest> ValidRequestAsync()
    {
        var response = await _client.GetFromJsonAsync<GeneralResponseDTO<VacunoCatalogsResponse>>(
            RequirementApiRoutes.VacunoCatalogos);
        var catalogs = response?.Data;
        catalogs.Should().NotBeNull("the tenant must expose vacuno catalogs");
        catalogs!.Granjas.Should().NotBeEmpty();

        var acquisition = catalogs.TiposAdquisicion.Single(x =>
            x.Code == BaseCatalogSeeder.TipoAdquisicionCode);
        var raza = catalogs.Razas.Single(x => x.Code == BaseCatalogSeeder.RazaCode);
        var color = catalogs.Colores.Single(x => x.Code == BaseCatalogSeeder.ColorCode);
        var sexo = catalogs.Sexos.Single(x => x.Code == BaseCatalogSeeder.SexoHembraCode);
        var granja = catalogs.Granjas.Single(x => x.Id == BaseCatalogSeeder.GranjaId);
        var utilizacion = catalogs.Utilizaciones.Single(x => x.Code == BaseCatalogSeeder.UtilizacionCode);

        return new CreateVacunoRequest(
            $"IT{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            "Vacuno Integracion",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
            acquisition.Code,
            raza.Code,
            color.Code,
            sexo.Code,
            null, null,
            granja.Id,
            null, null, null, null, null,
            acquisition.Code.Equals("COMPRA", StringComparison.OrdinalIgnoreCase) ? 100m : null,
            utilizacion.Code,
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Prueba de integracion");
    }

    public async Task<VacunoResponse> CreateAsync(CreateVacunoRequest request)
    {
        var response = await _client.PostAsJsonAsync(RequirementApiRoutes.Vacunos, request);
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create vacuno. Status: {response.StatusCode}. Error: {err}");
        }
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<VacunoResponse>>();
        body!.Data.Should().NotBeNull();
        _createdIds.Add(body.Data!.Id);
        return body.Data;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var id in _createdIds)
        {
            using var delete = new HttpRequestMessage(HttpMethod.Delete, RequirementApiRoutes.Vacuno(id))
            {
                Content = JsonContent.Create(new DeleteVacunoRequest("Limpieza de prueba de integracion"))
            };
            await _client.SendAsync(delete);
        }
    }
}
