using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

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
}
