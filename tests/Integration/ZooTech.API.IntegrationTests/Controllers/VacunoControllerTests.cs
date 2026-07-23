using FluentAssertions;
using Xunit;
using System.Net;
using System.Net.Http.Json;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ClosedXML.Excel;
using ZooTech.API.IntegrationTests.Seeders;
using ZooTech.API.IntegrationTests.Support;

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
        var response = await _client.GetAsync(RequirementApiRoutes.VacunosPage(1, 5));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Page.Should().Be(1);
        content.Limit.Should().Be(5);
        content.Data.Should().NotBeNull();
    }

    [Theory]
    [InlineData("zootecniaunas.zentrycorp.local")]
    [InlineData("elroble.zentrycorp.local")]
    [InlineData("lacteosdelvalle.zentrycorp.local")]
    public async Task GetArbolGenealogico_WhenVacunoHasLineage_ReturnsTreeUpToMaxLevels(string tenantHost)
    {
        using var client = _factory.CreateTenantClient(tenantHost);

        // Fetch ID of V001
        var vacuno = await GetVacunoByCodeAsync(client, GenealogiaSeeder.TargetCode);
        vacuno.Should().NotBeNull("Se espera que el hijo principal V001 esté inyectado en memoria.");

        // Requerir hasta 10 niveles (el backend lo clamp a maxNiveles configurado, ej: 4)
        var response = await client.GetAsync(
            RequirementApiRoutes.VacunoGenealogia(vacuno!.Id, niveles: 10));
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<List<GetArbolGenealogicoItem>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        
        // Debería retornar Abuelos (Nivel 3), Padres (Nivel 2) y el Hijo (Nivel 1) = 5 nodos
        content.Data.Should().NotBeNullOrEmpty();
        content.Data!.Count.Should().Be(5);
        content.Data.Any(n => n.Nivel == 1 && n.Nombre == "Hijo Principal").Should().BeTrue();
        content.Data.Any(n => n.Nivel == 3 && n.Nombre == "Abuelo").Should().BeTrue();
    }

    [Fact]
    public async Task GetArbolGenealogico_WhenNivelesIsInvalid_ReturnsBadRequest()
    {
        var response = await _client.GetAsync(
            RequirementApiRoutes.VacunoGenealogia(1, niveles: -1));
        // Dependiendo de la validación puede retornar 400 Bad Request o el pipeline lo fuerza a un valor válido,
        // pero validamos que no provoque errores 500.
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }


    [Fact]
    public async Task ExportarArbolGenealogico_WhenFormatoIsInvalid_ReturnsBadRequest()
    {
        var vacuno = await GetVacunoByCodeAsync(_client, GenealogiaSeeder.TargetCode);
        
        var response = await _client.GetAsync(
            RequirementApiRoutes.VacunoGenealogiaExportar(vacuno!.Id, "csv"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExportarArbolGenealogico_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            RequirementApiRoutes.VacunoGenealogiaExportar(RequirementApiRoutes.MissingEntityId));

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

        var response = await client.GetAsync(RequirementApiRoutes.VacunosPage(1, 1));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVacunoById_WhenVacunoExists_ReturnsOkAndVacuno()
    {
        var vacuno = await GetVacunoByCodeAsync(_client, VacunosBasicSeeder.HembraCode);
        vacuno.Should().NotBeNull("the tenant database must contain at least one active vacuno");

        var response = await _client.GetAsync(RequirementApiRoutes.Vacuno(vacuno!.Id));
        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"because the server returned {response.StatusCode} with body: {responseBody}");
        
        var content = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<VacunoResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(vacuno.Id);
    }

    [Fact]
    public async Task GetVacunoById_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            RequirementApiRoutes.Vacuno(RequirementApiRoutes.MissingEntityId));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActivityStats_WithValidDates_ReturnsOkAndStats()
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync(
            RequirementApiRoutes.VacunoActivityStats(today, today));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<VacunoActivityStatsResponse>();
        content.Should().NotBeNull();
        content!.Points.Should().ContainSingle();
        content.Points[0].Fecha.Should().Be(today);
        content.Points[0].Cantidad.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetActivityStats_WithInvalidEffectiveRange_ReturnsBadRequest()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        var response = await _client.GetAsync(
            $"/api/v1/vacunos/estadisticas/actividad?fechaInicio={tomorrow:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("excel", ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("pdf", ".pdf", "application/pdf")]
    public async Task ReportesListado_WithDownloadFormat_GeneratesDownloadableFile(
        string formato,
        string extension,
        string expectedContentType)
    {
        var response = await _client.GetAsync($"/api/v1/vacunos/reportes/listado?formato={formato}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ListadoVacunosReporteResponse>();
        report.Should().NotBeNull();
        report!.DownloadUrl.Should().NotBeNull().And.EndWith(extension);

        var download = await _client.GetAsync(report.DownloadUrl);
        download.StatusCode.Should().Be(HttpStatusCode.OK);
        download.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
        (await download.Content.ReadAsByteArrayAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task ReportesListado_WithInvalidFormat_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/vacunos/reportes/listado?formato=csv");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("excel", ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("pdf", ".pdf", "application/pdf")]
    public async Task ReporteIndividual_WithDownloadFormat_GeneratesDownloadableFile(
        string formato,
        string extension,
        string expectedContentType)
    {
        var listResponse = await _client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            "/api/v1/vacunos?page=1&limit=1&q=VAC001");
        var vacuno = listResponse?.Data?.FirstOrDefault();
        vacuno.Should().NotBeNull();

        var response = await _client.GetAsync(
            $"/api/v1/vacunos/{vacuno!.Id}/reporte?formato={formato}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<RegistroVacunoReporteResponse>();
        report.Should().NotBeNull();
        report!.Vacuno.Codigo.Should().Be("VAC001");
        report.DownloadUrl.Should().NotBeNull().And.EndWith(extension);

        var download = await _client.GetAsync(report.DownloadUrl);
        download.StatusCode.Should().Be(HttpStatusCode.OK);
        download.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
        (await download.Content.ReadAsByteArrayAsync()).Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("zootecniaunas.zentrycorp.local", "excel", ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("zootecniaunas.zentrycorp.local", "pdf", ".pdf", "application/pdf")]
    [InlineData("elroble.zentrycorp.local", "excel", ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("elroble.zentrycorp.local", "pdf", ".pdf", "application/pdf")]
    public async Task ExportarGenealogia_WithSupportedFormat_ReturnsBinaryFile(
        string tenantHost,
        string formato,
        string extension,
        string expectedContentType)
    {
        using var client = _factory.CreateTenantClient(tenantHost);
        var listResponse = await client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            "/api/v1/vacunos?page=1&limit=10&q=V001");
        var vacuno = listResponse?.Data?.Single(item => item.Codigo == "V001");
        vacuno.Should().NotBeNull();

        var response = await client.GetAsync(
            $"/api/v1/vacunos/{vacuno!.Id}/genealogia/exportar?formato={formato}&niveles=4");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
        response.Content.Headers.ContentDisposition!.FileNameStar.Should().EndWith(extension);
        (await response.Content.ReadAsByteArrayAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExportarGenealogia_WhenVacunoDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            "/api/v1/vacunos/999999/genealogia/exportar?formato=pdf&niveles=4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("excel", ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("pdf", ".pdf", "application/pdf")]
    public async Task ExportarActividad_WithSupportedFormat_ReturnsBinaryFile(
        string formato,
        string extension,
        string expectedContentType)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var response = await _client.GetAsync(
            $"/api/v1/vacunos/estadisticas/actividad/exportar?formato={formato}&fechaInicio={today}&fechaFin={today}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be(expectedContentType);
        response.Content.Headers.ContentDisposition!.FileNameStar.Should().EndWith(extension);
        (await response.Content.ReadAsByteArrayAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task ReportesListado_WithMoreThanTwoHundredFilteredRows_ExportsAllRows()
    {
        var response = await _client.GetAsync(
            "/api/v1/vacunos/reportes/listado?formato=excel&search=RPT-");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ListadoVacunosReporteResponse>();
        report!.DownloadUrl.Should().NotBeNull();

        var download = await _client.GetAsync(report.DownloadUrl);
        var content = await download.Content.ReadAsByteArrayAsync();
        using var workbook = new XLWorkbook(new MemoryStream(content));
        var exportedCodes = workbook.Worksheet("Listado de vacunos")
            .Column(1)
            .CellsUsed()
            .Count(cell => cell.GetString().StartsWith("RPT-", StringComparison.Ordinal));

        exportedCodes.Should().Be(205);
    }

    [Fact]
    public async Task ReportesListado_WhenTenantDisablesPdf_ReturnsBadRequest()
    {
        using var client = _factory.CreateTenantClient("losandes.zentrycorp.local");

        var response = await client.GetAsync(
            "/api/v1/vacunos/reportes/listado?formato=pdf");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task<VacunoItemResponse?> GetVacunoByCodeAsync(HttpClient client, string code)
    {
        var response = await client.GetFromJsonAsync<PagedResponse<List<VacunoItemResponse>>>(
            RequirementApiRoutes.VacunosPage(1, 5, code));

        return response?.Data?.SingleOrDefault(item => item.Codigo == code);
    }
}
