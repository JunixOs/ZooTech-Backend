using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Domain.Module_Celo.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetCelos;

public sealed class CeloItemDtoMapperTests
{
    [Fact]
    public void Map_CopiesAllFieldsFromCeloListItem()
    {
        var fechaHora = new DateTime(2026, 3, 10, 14, 30, 0);
        var celo = CeloListItem.Rehydrate(
            id: 1,
            codigo: "CEL-001",
            fechaHora: fechaHora,
            vacunoId: 42,
            vacunoCodigo: "VAC-042",
            nombreVacuno: "Manchada",
            observaciones: "Celo detectado por monta",
            caracteristicaCodes: ["FLUJO", "MUGIDO"]);
        var counts = new Dictionary<long, int> { [42] = 3 };

        var dto = CeloItemDtoMapper.Map(celo, counts);

        Assert.Equal(1, dto.Id);
        Assert.Equal("CEL-001", dto.CodigoRegistro);
        Assert.Equal(DateOnly.FromDateTime(fechaHora), dto.Fecha);
        Assert.Equal(TimeOnly.FromDateTime(fechaHora), dto.Hora);
        Assert.Equal("VAC-042", dto.CodigoVacuno);
        Assert.Equal("Manchada", dto.NombreVacuno);
        Assert.Equal(3, dto.VecesEnCelo);
        Assert.Equal("Celo detectado por monta", dto.Observaciones);
        Assert.Equal(["FLUJO", "MUGIDO"], dto.CaracteristicaCodes);
    }

    [Fact]
    public void Map_WhenVacunoHasNoCounts_DefaultsVecesEnCeloToOne()
    {
        var celo = CeloListItem.Rehydrate(
            id: 2,
            codigo: "CEL-002",
            fechaHora: DateTime.Now,
            vacunoId: 99,
            vacunoCodigo: "VAC-099",
            nombreVacuno: "Overa",
            observaciones: null,
            caracteristicaCodes: []);
        var counts = new Dictionary<long, int>();

        var dto = CeloItemDtoMapper.Map(celo, counts);

        Assert.Equal(1, dto.VecesEnCelo);
        Assert.Null(dto.Observaciones);
        Assert.Empty(dto.CaracteristicaCodes);
    }
}
