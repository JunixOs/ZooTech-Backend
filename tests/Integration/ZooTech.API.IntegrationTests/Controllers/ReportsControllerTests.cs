using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;

namespace ZooTech.API.IntegrationTests.Controllers;

public class ReportsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReportsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(Skip = "Requires a live Redis / DB instance reachable from the CI agent.")]
    public async Task GetAnimalsReport_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/reports/animals?fechaInicio=2023-01-01&fechaFin=2023-06-01");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires a live Redis / DB instance reachable from the CI agent.")]
    public async Task DownloadAnimalsPdf_ReturnsPdfFile()
    {
        var response = await _client.GetAsync("/api/reports/animals/pdf?fechaInicio=2023-01-01&fechaFin=2023-06-01");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
    }
}
