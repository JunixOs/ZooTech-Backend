using Microsoft.AspNetCore.Http;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Vacuno;

public sealed class RegistrarVacunoMappingTests
{
    [Fact]
    public void ToCommand_NormalizaCodigoYMapeaCamposDelFrontend()
    {
        var request = RequestValido();

        var command = VacunoMapper.ToCommand(request);

        Assert.Equal("VACA001", command.Codigo);
        Assert.Equal(2, command.IdTipoAdquisicion);
        Assert.Equal(3, command.IdRaza);
        Assert.Equal("Angus", command.Raza);
        Assert.Equal(2, command.IdSexo);
        Assert.Equal("hembra", command.Sexo);
        Assert.Equal(1, command.IdTipoUtilizacion);
        Assert.Equal("produccion_leche", command.AptoPara);
        Assert.Equal("TORO001", command.CodigoPadre);
        Assert.Equal("VACA002", command.CodigoMadre);
    }

    [Fact]
    public void ToCommand_AsociaFotoComoStream()
    {
        var request = RequestValido();
        request.Foto = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "foto", "vaca.png");

        var command = VacunoMapper.ToCommand(request);

        Assert.NotNull(command.FotoStream);
        Assert.Equal("vaca.png", command.FotoNombreOriginal);
    }

    private static RegistrarVacunoRequest RequestValido() => new()
    {
        Codigo = "vaca001",
        Nombre = "Lola",
        FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
        AdquisicionPor = "compra",
        PrecioCompra = 120,
        Raza = "Angus",
        Color = "Negro",
        Sexo = "hembra",
        CodigoPadre = "toro001",
        CodigoMadre = "vaca002",
        Granja = "Granja Norte",
        Distrito = "Tocache",
        Departamento = "San Martin",
        Provincia = "Tocache",
        AptoPara = "produccion_leche",
        FechaEspecificacion = DateOnly.FromDateTime(DateTime.Today),
        Observaciones = "Sin novedades"
    };
}
