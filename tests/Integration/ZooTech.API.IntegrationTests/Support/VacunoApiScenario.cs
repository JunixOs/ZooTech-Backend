using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
            $"{RequirementApiRoutes.Vacunos}/catalogos");
        var catalogs = response?.Data;
        catalogs.Should().NotBeNull("the tenant must expose vacuno catalogs");
        catalogs!.Granjas.Should().NotBeEmpty();

        var acquisition = catalogs.TiposAdquisicion
            .FirstOrDefault(x => !x.Code.Equals("COMPRA", StringComparison.OrdinalIgnoreCase))
            ?? catalogs.TiposAdquisicion.First();

        return new CreateVacunoRequest(
            $"IT{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            "Vacuno Integracion",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
            acquisition.Code,
            catalogs.Razas.First().Code,
            catalogs.Colores.First().Code,
            catalogs.Sexos.First().Code,
            null, null,
            catalogs.Granjas.First().Id,
            null, null, null, null, null,
            acquisition.Code.Equals("COMPRA", StringComparison.OrdinalIgnoreCase) ? 100m : null,
            catalogs.Utilizaciones.FirstOrDefault()?.Code,
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
