using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZooTech.API.IntegrationTests.Support;

namespace ZooTech.API.IntegrationTests.ReporteVacuno;

[TestClass]
public sealed class ReporteVacunoFlowIntegrationTests
{
    [TestMethod]
    public async Task ExcelFlow_ShouldListFilterOpenDetailGenerateDownloadUrlAndExposeFile()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var listado = await GetJsonAsync(client, "/vacunos/reportes/listado?fechaDesde=2026-01-01&fechaHasta=2026-12-31&q=VACA001&formato=json&page=1&limit=20");
        Assert.AreEqual("VACA001", listado.RootElement.GetProperty("data")[0].GetProperty("codigo").GetString());

        var detalle = await GetJsonAsync(client, "/vacunos/1/reporte?formato=json");
        Assert.AreEqual("VACA001", detalle.RootElement.GetProperty("vacuno").GetProperty("codigo").GetString());
        Assert.AreEqual(JsonValueKind.Null, detalle.RootElement.GetProperty("downloadUrl").ValueKind);

        var excel = await GetJsonAsync(client, "/vacunos/1/reporte?formato=excel");
        var downloadUrl = excel.RootElement.GetProperty("downloadUrl").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(downloadUrl));
        StringAssert.EndsWith(downloadUrl!, ".xlsx");

        var file = await client.GetAsync(downloadUrl);
        Assert.AreEqual(HttpStatusCode.OK, file.StatusCode);
        Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task PdfFlow_ShouldListFilterOpenDetailGenerateDownloadUrlAndExposeFile()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var listado = await GetJsonAsync(client, "/vacunos/reportes/listado?fechaDesde=2026-01-01&fechaHasta=2026-12-31&q=Luna&formato=json&page=1&limit=20");
        Assert.AreEqual(1, listado.RootElement.GetProperty("data").GetArrayLength());

        var pdf = await GetJsonAsync(client, "/vacunos/1/reporte?formato=pdf");
        var downloadUrl = pdf.RootElement.GetProperty("downloadUrl").GetString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(downloadUrl));
        StringAssert.EndsWith(downloadUrl!, ".pdf");

        var file = await client.GetAsync(downloadUrl);
        Assert.AreEqual(HttpStatusCode.OK, file.StatusCode);
        Assert.AreEqual("application/pdf", file.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task DefaultDateRange_ShouldReturnOnlyRecordsInsideTheLastThirtyDays()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var response = await GetJsonAsync(client, "/vacunos/reportes/listado?formato=json&page=1&limit=20");

        Assert.AreEqual(1, response.RootElement.GetProperty("data").GetArrayLength());
        Assert.AreEqual("VACA001", response.RootElement.GetProperty("data")[0].GetProperty("codigo").GetString());
        Assert.AreEqual("2026-04-28", response.RootElement.GetProperty("filtros").GetProperty("fechaDesde").GetString());
        Assert.AreEqual("2026-05-28", response.RootElement.GetProperty("filtros").GetProperty("fechaHasta").GetString());
    }

    [TestMethod]
    public async Task Pagination_ShouldReturnExpectedPageAndTotal()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var response = await GetJsonAsync(client, "/vacunos/reportes/listado?fechaDesde=2026-01-01&fechaHasta=2026-12-31&formato=json&page=2&limit=1");

        Assert.AreEqual(1, response.RootElement.GetProperty("data").GetArrayLength());
        Assert.AreEqual(2, response.RootElement.GetProperty("resumen").GetProperty("totalVacunos").GetInt32());
        Assert.AreEqual("VACA002", response.RootElement.GetProperty("data")[0].GetProperty("codigo").GetString());
    }

    [TestMethod]
    public async Task EmptyResult_ShouldReturnEmptyDataAndZeroTotal()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var response = await GetJsonAsync(client, "/vacunos/reportes/listado?fechaDesde=2026-01-01&fechaHasta=2026-12-31&q=NO_EXISTE&formato=json&page=1&limit=20");

        Assert.AreEqual(0, response.RootElement.GetProperty("data").GetArrayLength());
        Assert.AreEqual(0, response.RootElement.GetProperty("resumen").GetProperty("totalVacunos").GetInt32());
    }

    [TestMethod]
    public async Task InvalidDateRange_ShouldReturnBadRequestWithValidationError()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/vacunos/reportes/listado?fechaDesde=2026-05-30&fechaHasta=2026-05-20&formato=json");
        var error = await response.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("VALIDATION_ERROR", error!.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [TestMethod]
    public async Task Detail_WhenVacunoDoesNotExist_ShouldReturnNotFound()
    {
        await using var factory = new ReporteVacunoApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/vacunos/999/reporte?formato=json");
        var error = await response.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("VACUNO_NOT_FOUND", error!.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [TestMethod]
    public async Task Download_WhenBackendFails_ShouldReturnInternalServerErrorWithoutDownloadUrl()
    {
        await using var factory = new ReporteVacunoApiFactory(forceDownloadFailure: true);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/vacunos/1/reporte?formato=excel");
        var error = await response.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.AreEqual("INTERNAL_SERVER_ERROR", error!.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> GetJsonAsync(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, json);
        return JsonDocument.Parse(json);
    }
}
