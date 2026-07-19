using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

namespace ZooTech.API.IntegrationTests.Controllers;

public class VacunoControllerTests : IClassFixture<ZooTechApiFactory>
{
    private readonly HttpClient _client;
    private readonly ZooTechApiFactory _factory;

    public VacunoControllerTests(ZooTechApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateTenantClient();
    }

    [Fact]
    public async Task ListarVacunos_ReturnsOk_AndPagedResponse()
    {
        var response = await _client.GetAsync("/api/v1/vacunos?page=1&limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Page.Should().Be(1);
        content.Limit.Should().Be(5);
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetArbolGenealogico_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoExists_ReturnsExcelFile()
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            "/api/v1/vacunos?page=1&limit=1");
        var vacuno = listResponse?.Data?.FirstOrDefault();
        vacuno.Should().NotBeNull("the tenant database must contain at least one active vacuno");

        var response = await _client.GetAsync($"/api/v1/vacunos/{vacuno!.Id}/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenFormatoIsPdf_ReturnsPdfFile()
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            "/api/v1/vacunos?page=1&limit=1");
        var vacuno = listResponse?.Data?.FirstOrDefault();
        vacuno.Should().NotBeNull("the tenant database must contain at least one active vacuno");

        var response = await _client.GetAsync($"/api/v1/vacunos/{vacuno!.Id}/genealogia/exportar?formato=pdf");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("zootecniaunas.zentrycorp.local")]
    [InlineData("elroble.zentrycorp.local")]
    [InlineData("lacteosdelvalle.zentrycorp.local")]
    [InlineData("losandes.zentrycorp.local")]
    public async Task ListarVacunos_ForConfiguredTenant_ReturnsOk(string tenantHost)
    {
        using var client = _factory.CreateTenantClient(tenantHost);

        var response = await client.GetAsync("/api/v1/vacunos?page=1&limit=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVacunoById_WhenVacunoExists_ReturnsOkAndVacuno()
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            "/api/v1/vacunos?page=1&limit=1");
        var vacuno = listResponse?.Data?.FirstOrDefault();
        vacuno.Should().NotBeNull("the tenant database must contain at least one active vacuno");

        var response = await _client.GetAsync($"/api/v1/vacunos/{vacuno!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GetVacunoById Response: {json}");
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<VacunoResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(vacuno.Id);
    }

    [Fact]
    public async Task GetVacunoById_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActivityStats_WithValidDates_ReturnsOkAndStats()
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/v1/vacunos/estadisticas/actividad?fechaInicio={today}&fechaFin={today}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GetActivityStats Response: {json}");
        var content = await response.Content.ReadFromJsonAsync<VacunoActivityStatsResponse>();
        content.Should().NotBeNull();
        content!.Points.Should().NotBeNull();
    }
}
