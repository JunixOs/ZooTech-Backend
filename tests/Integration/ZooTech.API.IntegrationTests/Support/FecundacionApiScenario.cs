using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Support;

internal sealed class FecundacionApiScenario
{
    private readonly HttpClient _client;

    public FecundacionApiScenario(HttpClient client) => _client = client;

    public async Task<CreateFecundacionResponse> CreateAsync()
    {
        var data = await GetFormDataAsync();
        using var donorId = JsonDocument.Parse(data.VacunoDonanteId.ToString());
        var request = new CreateFecundacionRequest(
            TipoFecundacion: data.TipoFecundacionCode,
            VacunoReceptorId: data.VacunoReceptorId,
            MachoODonante: donorId.RootElement.Clone(),
            MachoExterno: false,
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.UtcNow),
            Responsable: "Responsable Test Integracion",
            Resultado: data.ResultadoCode,
            CodigoSemen: null,
            CodigoEmbrion: null,
            Observaciones: "Creado por prueba de integracion");

        var response = await _client.PostAsJsonAsync(RequirementApiRoutes.Fecundaciones, request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content
            .ReadFromJsonAsync<GeneralResponseDTO<CreateFecundacionResponse>>();
        content?.Data.Should().NotBeNull();
        return content!.Data!;
    }

    private async Task<FecundacionFormData> GetFormDataAsync()
    {
        var options = await _client
            .GetFromJsonAsync<GeneralResponseDTO<FecundacionOptionsResponse>>(
                RequirementApiRoutes.FecundacionOpciones);
        var hembras = await _client
            .GetFromJsonAsync<GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>>(
                RequirementApiRoutes.FecundacionVacunos(
                    "HEMBRA",
                    VacunosBasicSeeder.HembraCode,
                    soloDisponibles: true));
        var machos = await _client
            .GetFromJsonAsync<GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>>(
                RequirementApiRoutes.FecundacionVacunos(
                    "MACHO",
                    VacunosBasicSeeder.MachoCode));

        options?.Data.Should().NotBeNull();
        var tipo = options!.Data!.Tipos.Single(item =>
            item.Code == FecundacionCatalogSeeder.TipoApiCode);
        var resultado = options.Data.Resultados.Single(item =>
            item.Code == FecundacionCatalogSeeder.ResultadoApiCode);
        var receptor = hembras?.Data?.SingleOrDefault(item =>
            item.Codigo == VacunosBasicSeeder.HembraCode);
        var donante = machos?.Data?.SingleOrDefault(item =>
            item.Codigo == VacunosBasicSeeder.MachoCode);

        receptor.Should().NotBeNull("the tenant must contain an available female vacuno");
        donante.Should().NotBeNull("the tenant must contain an active male vacuno");

        return new FecundacionFormData(
            tipo.Code,
            resultado.Code,
            receptor!.Id,
            donante!.Id);
    }

    private sealed record FecundacionFormData(
        string TipoFecundacionCode,
        string ResultadoCode,
        long VacunoReceptorId,
        long VacunoDonanteId);
}
