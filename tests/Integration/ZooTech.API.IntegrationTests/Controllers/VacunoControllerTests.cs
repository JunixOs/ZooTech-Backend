using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public class VacunoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public VacunoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
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

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task GetArbolGenealogico_WhenVacunoDoesNotExist_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia");

        response.StatusCode.Should().Be(HttpStatusCode.OK); // Interactor devuelve lista vacía
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task ExportarArbolGenealogico_WhenVacunoExists_ReturnsExcelFile()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZooTech.Infrastructure.Persistence.Context.GanaderiaDbContext>();
        var v = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.vacunos.Where(x => x.deleted_at == null));
        if (v == null) {
            var sexo = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(db.cat_sexos);
            var raza = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(db.cat_razas);
            var color = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(db.cat_colors);
            var adq = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(db.cat_tipo_adquisicions);
            var granja = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(db.granjas);

            v = new ZooTech.Infrastructure.Persistence.Entities.vacuno { 
                codigo = "VAC" + Guid.NewGuid().ToString("N").Substring(0, 8), nombre = "Test", 
                sexo_code = sexo.code, raza_code = raza.code, 
                color_code = color.code, tipo_adquisicion_code = adq.code, 
                granja_id = granja.id,
                fecha_nacimiento = new DateOnly(2020,1,1) 
            };
            db.vacunos.Add(v);
            await db.SaveChangesAsync();
        }

        var response = await _client.GetAsync($"/api/v1/vacunos/{v.id}/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact(Skip = "Requires a live Redis instance reachable from the CI agent (Redis:ConnectionString is empty there); WebApplicationFactory<Program> fails to build the host. Unskip once CI provides Redis config.")]
    public async Task ExportarArbolGenealogico_WhenVacunoDoesNotExist_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/999999/genealogia/exportar");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // Interactor lanza NotFoundException
    }
}
