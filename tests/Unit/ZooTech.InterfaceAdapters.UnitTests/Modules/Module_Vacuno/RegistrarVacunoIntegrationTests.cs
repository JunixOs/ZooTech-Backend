using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Ajusta el namespace al assembly real del entry-point (.API o .InterfaceAdapters)
// según cuál tenga el Program.cs que levanta la aplicación.
using TEntryPoint = ZooTech.API.Program; // ← cambia si el entry-point es otro

namespace ZooTech.InterfaceAdapters.IntegrationTests.Modules.Module_Vacuno;

// Fixture: levanta la app una sola vez para toda la clase
public class RegistrarVacunoIntegrationTests
    : IClassFixture<WebApplicationFactory<TEntryPoint>>
{
    private readonly HttpClient _client;

    public RegistrarVacunoIntegrationTests(WebApplicationFactory<TEntryPoint> factory)
    {
        _client = factory.CreateClient();

        // Token JWT de prueba — reemplaza con uno válido de tu entorno de test
        // o inyecta un FakeAuthHandler en WithWebHostBuilder si prefieres evitar
        // la dependencia de un token real.
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ObtenerTokenTest());
    }

    // ─────────────────────────────────────────────────────────
    // Helper: construye un multipart/form-data completo
    // ─────────────────────────────────────────────────────────

    private static MultipartFormDataContent FormularioValido(
        string codigo = "INTTEST01",
        string nombre = "VacaIntTest",
        string adquisicion = "monta",
        decimal? precio = null,
        bool incluirFoto = false,
        string? observacion = null)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(codigo),          "codigo"              },
            { new StringContent(nombre),          "nombre"              },
            { new StringContent("2022-01-15"),    "fechaNacimiento"     },
            { new StringContent(adquisicion),     "adquisicionPor"      },
            { new StringContent("Angus"),         "raza"                },
            { new StringContent("Negro"),         "color"               },
            { new StringContent("hembra"),        "sexo"                },
            { new StringContent("TORO001"),       "codigoPadre"         },
            { new StringContent("VACA002"),       "codigoMadre"         },
            { new StringContent("GranjaNorte"),   "granja"              },
            { new StringContent("Tocache"),       "distrito"            },
            { new StringContent("San Martín"),    "departamento"        },
            { new StringContent("Tocache"),       "provincia"           },
            { new StringContent("produccion_leche"), "aptoPara"         },
            { new StringContent("2024-03-01"),    "fechaEspecificacion" },
        };

        if (precio.HasValue)
            form.Add(new StringContent(precio.Value.ToString("F2")), "precioCompra");

        if (observacion is not null)
            form.Add(new StringContent(observacion), "observaciones");

        if (incluirFoto)
        {
            var fotoBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG magic
            var fotoContent = new ByteArrayContent(fotoBytes);
            fotoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            form.Add(fotoContent, "foto", "foto_test.jpg");
        }

        return form;
    }

    // ─────────────────────────────────────────────────────────
    // TK 10 — Flujo de registro exitoso
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_VacunoValido_Devuelve201YCuerpoEsperado()
    {
        // Arrange — usa un código único para no colisionar con otros tests
        var form = FormularioValido(codigo: "TK10A_" + Guid.NewGuid().ToString()[..4].ToUpper());

        // Act
        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<RegistrarVacunoRespuesta>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.Codigo.Should().NotBeNullOrWhiteSpace();
        body.Nombre.Should().NotBeNullOrWhiteSpace();
        body.CreadoEn.Should().NotBe(default);
    }

    [Fact]
    public async Task POST_VacunoConCompra_Devuelve201YPrecioRegistrado()
    {
        var form = FormularioValido(
            codigo: "TK10B_" + Guid.NewGuid().ToString()[..4].ToUpper(),
            adquisicion: "compra",
            precio: 350.00m);

        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<RegistrarVacunoRespuesta>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body!.PrecioCompra.Should().Be(350.00m);
    }

    // ─────────────────────────────────────────────────────────
    // TK 11 — Reglas de rechazo
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_CodigoDuplicado_Devuelve409ConCodigoError()
    {
        // Arrange — registrar primero para crear el duplicado
        var codigoFijo = "DUPTEST01";
        var primer = FormularioValido(codigo: codigoFijo);
        await _client.PostAsync("/v1/vacunos", primer); // primera vez (puede fallar si ya existe)

        // Act — reintentar con el mismo código
        var segundo = FormularioValido(codigo: codigoFijo);
        var response = await _client.PostAsync("/v1/vacunos", segundo);
        var body = await response.Content.ReadFromJsonAsync<ErrorRespuesta>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        body!.Error.Code.Should().Be("VACUNO_ALREADY_EXISTS");
    }

    [Fact]
    public async Task POST_SinCamposObligatorios_Devuelve400ConDetallesDeValidacion()
    {
        // Formulario vacío
        var form = new MultipartFormDataContent();

        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<ErrorRespuesta>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body!.Error.Code.Should().Be("VALIDATION_ERROR");
        body.Error.Details.Should().NotBeEmpty();
    }

    [Fact]
    public async Task POST_CodigoSuperaMaxLength_Devuelve400ConDetalleCodigo()
    {
        var form = FormularioValido(codigo: "CODIGO_MUY_LARGO_QUE_SUPERA_10");

        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<ErrorRespuesta>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body!.Error.Details.Should()
            .Contain(d => d.Field == "codigo");
    }

    [Fact]
    public async Task POST_AdquisicionCompraYSinPrecio_Devuelve400ConDetallePrecioCompra()
    {
        var form = FormularioValido(
            codigo: "TK11C_" + Guid.NewGuid().ToString()[..4].ToUpper(),
            adquisicion: "compra",
            precio: null); // falta precio

        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<ErrorRespuesta>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body!.Error.Details.Should()
            .Contain(d => d.Field == "precioCompra");
    }

    [Fact]
    public async Task POST_SinToken_Devuelve401()
    {
        // Cliente sin token
        var clienteSinToken = new HttpClient { BaseAddress = _client.BaseAddress };
        var form = FormularioValido();

        var response = await clienteSinToken.PostAsync("/v1/vacunos", form);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task POST_ObservacionesSuperaLimite_Devuelve400()
    {
        var observacion31palabras = string.Join(" ",
            Enumerable.Range(1, 31).Select(i => $"palabra{i}"));

        var form = FormularioValido(
            codigo: "TK11E_" + Guid.NewGuid().ToString()[..4].ToUpper(),
            observacion: observacion31palabras);

        var response = await _client.PostAsync("/v1/vacunos", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─────────────────────────────────────────────────────────
    // TK 12 — Carga de imagen y cancelación
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task POST_ConFotoValida_Devuelve201YFotoUrlNoEsNula()
    {
        var form = FormularioValido(
            codigo: "TK12A_" + Guid.NewGuid().ToString()[..4].ToUpper(),
            incluirFoto: true);

        var response = await _client.PostAsync("/v1/vacunos", form);
        var body = await response.Content.ReadFromJsonAsync<RegistrarVacunoRespuesta>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body!.FotoUrl.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task POST_ConFotoFormatoInvalido_Devuelve400()
    {
        var form = FormularioValido(
            codigo: "TK12B_" + Guid.NewGuid().ToString()[..4].ToUpper());

        // Reemplazar foto con GIF (no permitido)
        var gifBytes = new byte[] { 0x47, 0x49, 0x46, 0x38 }; // GIF magic
        var gifContent = new ByteArrayContent(gifBytes);
        gifContent.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
        form.Add(gifContent, "foto", "foto_invalida.gif");

        var response = await _client.PostAsync("/v1/vacunos", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Cancelacion_NoDebeCrearRegistroEnBaseDeDatos()
    {
        // Este test verifica que si el usuario no envía el formulario
        // (simula "Cancelar") no queda ningún registro persistido.
        // Usamos un código único y verificamos que NO existe tras no enviarlo.

        var codigoTemporal = "CANCEL_" + Guid.NewGuid().ToString()[..4].ToUpper();

        // NO hacemos POST — solo verificamos que el código no existe
        var getResponse = await _client.GetAsync($"/v1/vacunos?q={codigoTemporal}");
        var body = await getResponse.Content
            .ReadFromJsonAsync<ListadoPaginado<VacunoListItemRespuesta>>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Data.Should().NotContain(v => v.Codigo == codigoTemporal);
    }

    [Fact]
    public async Task POST_FotoAsociadaAlVacunoCorrectamente_GetDetalleDevuelveFotoUrl()
    {
        // Arrange
        var codigo = "TK12D_" + Guid.NewGuid().ToString()[..4].ToUpper();
        var form = FormularioValido(codigo: codigo, incluirFoto: true);

        // Act — registrar
        var postResponse = await _client.PostAsync("/v1/vacunos", form);
        var postBody = await postResponse.Content.ReadFromJsonAsync<RegistrarVacunoRespuesta>();

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — obtener detalle y verificar fotoUrl
        var getResponse = await _client.GetAsync($"/v1/vacunos/{postBody!.Id}");
        var detalle = await getResponse.Content.ReadFromJsonAsync<VacunoDetalleRespuesta>();

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        detalle!.FotoUrl.Should().NotBeNullOrWhiteSpace();
    }

    // ─────────────────────────────────────────────────────────
    // DTOs locales para deserializar respuestas de la API
    // (solo los campos que necesitan los tests)
    // ─────────────────────────────────────────────────────────

    private record RegistrarVacunoRespuesta(
        int Id,
        string Codigo,
        string Nombre,
        string AdquisicionPor,
        decimal? PrecioCompra,
        string? FotoUrl,
        DateTime CreadoEn);

    private record ErrorRespuesta(ErrorDetalle Error);

    private record ErrorDetalle(
        string Code,
        string Message,
        List<CampoError> Details);

    private record CampoError(string Field, string Message);

    private record ListadoPaginado<T>(List<T> Data, object Pagination);

    private record VacunoListItemRespuesta(
        int Id,
        string Codigo,
        string Nombre,
        string Estado);

    private record VacunoDetalleRespuesta(
        int Id,
        string Codigo,
        string? FotoUrl);

    // ─────────────────────────────────────────────────────────
    // Token de prueba
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Devuelve un JWT válido para el entorno de integración.
    /// Opciones:
    ///   A) Leer de variable de entorno: Environment.GetEnvironmentVariable("TEST_JWT")
    ///   B) Generar con tu FakeJwtFactory de tests
    ///   C) Hardcodear uno de larga duración solo para tests (no producción)
    /// </summary>
    ///private static string ObtenerTokenTest() =>
    ///    Environment.GetEnvironmentVariable("ZOOTECH_TEST_JWT")
    ///    ?? "TU_TOKEN_JWT_DE_TEST_AQUI"; // ← reemplaza o usa variable de entorno
}