using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public class VacunoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VacunoControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ListarVacunos_ReturnsOk_AndPagedResponse()
    {
        var response = await _client.GetAsync("/api/v1/vacuno?page=1&limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Page.Should().Be(1);
        content.Limit.Should().Be(5);
        content.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetArbolGenealogico_WhenVacunoDoesNotExist_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/vacuno/999999/genealogia");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoExists_ReturnsExcelFile()
    {
        var response = await _client.GetAsync("/api/v1/vacuno/1/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoDoesNotExist_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/vacuno/999999/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
