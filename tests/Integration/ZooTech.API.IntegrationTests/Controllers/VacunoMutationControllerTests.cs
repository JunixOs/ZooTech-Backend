using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ZooTech.API.IntegrationTests.Support;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Models;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.API.IntegrationTests.Controllers;

public sealed class VacunoMutationControllerTests : IClassFixture<ZooTechApiFactory>
{
    private readonly HttpClient _client;

    public VacunoMutationControllerTests(ZooTechApiFactory factory)
    {
        _client = factory.CreateTenantClient();
    }

    [Fact]
    public async Task RegistrarVacuno_ValidRequest_PersistsAndReturnsCreatedResource()
    {
        await using var scenario = new VacunoApiScenario(_client);
        var request = await scenario.ValidRequestAsync();

        var created = await scenario.CreateAsync(request);
        var stored = await _client.GetFromJsonAsync<GeneralResponseDTO<VacunoResponse>>(
            RequirementApiRoutes.Vacuno(created.Id));

        stored!.Data!.Codigo.Should().Be(request.Codigo);
        stored.Data.Nombre.Should().Be(request.Nombre);
    }

    [Fact]
    public async Task RegistrarVacuno_DuplicateCodigo_ReturnsConflict()
    {
        await using var scenario = new VacunoApiScenario(_client);
        var request = await scenario.ValidRequestAsync();
        await scenario.CreateAsync(request);

        var response = await _client.PostAsJsonAsync(RequirementApiRoutes.Vacunos, request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RegistrarVacuno_MissingRequiredFields_ReturnsStructuredBadRequest()
    {
        await using var scenario = new VacunoApiScenario(_client);
        var request = await scenario.ValidRequestAsync() with { Codigo = "", Nombre = "" };

        var response = await _client.PostAsJsonAsync(RequirementApiRoutes.Vacunos, request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponseModel>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.Error.FieldErrors.Should().Contain(x => x.Field == "codigo");
        error.Error.FieldErrors.Should().Contain(x => x.Field == "nombre");
    }

    [Fact]
    public async Task EditarVacuno_ValidRequest_UpdatesEditableFieldsAndPreservesCodigo()
    {
        await using var scenario = new VacunoApiScenario(_client);
        var create = await scenario.ValidRequestAsync();
        var created = await scenario.CreateAsync(create);
        var update = ToUpdate(create) with { Nombre = "Vacuno Editado", Observaciones = "Edicion integrada" };

        var response = await _client.PatchAsJsonAsync(RequirementApiRoutes.Vacuno(created.Id), update);
        var body = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<VacunoResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Data!.Codigo.Should().Be(create.Codigo);
        body.Data.Nombre.Should().Be(update.Nombre);
    }

    [Fact]
    public async Task EditarVacuno_ImmutableDateChanged_ReturnsBadRequest()
    {
        await using var scenario = new VacunoApiScenario(_client);
        var create = await scenario.ValidRequestAsync();
        var created = await scenario.CreateAsync(create);
        var update = ToUpdate(create) with { FechaNacimiento = create.FechaNacimiento.AddDays(1) };

        var response = await _client.PatchAsJsonAsync(RequirementApiRoutes.Vacuno(created.Id), update);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static UpdateVacunoRequest ToUpdate(CreateVacunoRequest request) => new(
        request.Nombre, request.FechaNacimiento, request.TipoAdquisicionCode,
        request.RazaCode, request.ColorCode, request.SexoCode,
        request.CodigoPadre, request.CodigoMadre, request.GranjaId,
        request.Granja, request.Distrito, request.Departamento, request.Provincia,
        request.CodigoDistrito, request.PrecioCompra, request.AptoPara,
        request.FechaEspecificacion, request.Observaciones);
}
