using System.Text.Json;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Fecundacion;

public sealed class FecundacionMapperTests
{
    [Fact]
    public void ToContractTipo_ShouldMapInternalCodeToContractEnum()
    {
        var result = FecundacionMapper.ToContractTipo("INSEMINACION_ARTIFICIAL");

        Assert.Equal("inseminacion_artificial", result);
    }

    [Fact]
    public void ToInternalTipo_ShouldMapContractEnumToInternalCode()
    {
        var result = FecundacionMapper.ToInternalTipo("inseminacion_artificial");

        Assert.Equal("INSEMINACION_ARTIFICIAL", result);
    }

    [Fact]
    public void ToCommand_ShouldMapNumericMachoODonanteAsInternalDonor()
    {
        using var json = JsonDocument.Parse("9");
        var request = BuildRequest(
            tipoFecundacion: "inseminacion_artificial",
            machoODonante: json.RootElement.Clone(),
            machoExterno: false);

        var command = FecundacionMapper.ToCommand(request, BuildCurrent(), 1);

        Assert.Equal("INSEMINACION_ARTIFICIAL", command.TipoFecundacionCode);
        Assert.Equal("INTERNO", command.TipoDonante);
        Assert.Equal(9, command.VacunoDonanteId);
        Assert.Null(command.ExternoDonanteNombre);
    }

    [Fact]
    public void ToCommand_ShouldMapTextMachoODonanteAsExternalDonor()
    {
        using var json = JsonDocument.Parse("\"Toro externo Don Miguel\"");
        var request = BuildRequest(
            machoODonante: json.RootElement.Clone(),
            machoExterno: true);

        var command = FecundacionMapper.ToCommand(request, BuildCurrent(), 1);

        Assert.Equal("EXTERNO", command.TipoDonante);
        Assert.Null(command.VacunoDonanteId);
        Assert.Equal("Toro externo Don Miguel", command.ExternoDonanteNombre);
    }

    [Fact]
    public void ToCommand_ShouldUseEnEspera_WhenCurrentRecordHasNoStateHistory()
    {
        var request = BuildRequest();

        var command = FecundacionMapper.ToCommand(
            request,
            BuildCurrent(estadoFecundacionCode: string.Empty),
            1);

        Assert.Equal("EN_ESPERA", command.EstadoFecundacionCode);
    }

    [Fact]
    public void ToResponse_ShouldExposeEnEspera_WhenCurrentRecordHasNoStateHistory()
    {
        var response = FecundacionMapper.ToResponse(
            BuildCurrent(estadoFecundacionCode: string.Empty));

        Assert.Equal("en_espera", response.EstadoFecundacion);
    }

    [Theory]
    [InlineData("en_espera", "EN_ESPERA")]
    [InlineData("gestante", "GESTANTE")]
    [InlineData("vacia", "VACIA")]
    public void ToInternalEstado_ShouldMapSupportedCatalogStates(
        string contractCode,
        string expectedCode)
    {
        Assert.Equal(expectedCode, FecundacionMapper.ToInternalEstado(contractCode));
    }

    private static UpdateFecundacionRequest BuildRequest(
        string? tipoFecundacion = null,
        JsonElement? machoODonante = null,
        bool? machoExterno = null)
        => new(
            TipoFecundacion: tipoFecundacion,
            VacunoReceptorId: 7,
            MachoODonante: machoODonante,
            MachoExterno: machoExterno,
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.Today),
            Responsable: "Dr. Perez",
            Resultado: "pendiente_confirmacion",
            EstadoFecundacion: null,
            CodigoSemen: "SEM-001",
            CodigoEmbrion: null,
            Observaciones: "Sin novedades");

    private static GetFecundacionForEditOutput BuildCurrent(
        string estadoFecundacionCode = "EN_ESPERA")
        => new(
            Id: 1,
            Codigo: "FEC001",
            TipoFecundacionCode: "MONTA_NATURAL",
            VacunoReceptorId: 7,
            VacunoReceptorCodigo: "VACA001",
            VacunoReceptorNombre: "Luna",
            TipoDonante: "INTERNO",
            VacunoDonanteId: 3,
            VacunoDonanteCodigo: "TORO003",
            VacunoDonanteNombre: "Max",
            ExternoDonanteId: null,
            ExternoDonanteNombre: null,
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.Today),
            ResponsableNombre: "Dr. Perez",
            ResultadoCode: "PENDIENTE",
            EstadoFecundacionCode: estadoFecundacionCode,
            ObservacionesVeterinarias: null,
            CodigoSemen: null,
            CodigoEmbrion: null,
            CreadoEn: DateTime.UtcNow,
            ActualizadoEn: DateTime.UtcNow);
}
