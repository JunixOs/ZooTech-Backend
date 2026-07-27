using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public class CeloItemResponse
{
    public long Id { get; set; }

    public string CodigoRegistro { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string CodigoVacuno { get; set; } = null!;
    public string NombreVacuno { get; set; } = null!;

    public int VecesEnCelo { get; set; }
    public string? Observaciones { get; set; }
    public IReadOnlyList<string> CaracteristicaCodes { get; set; } = [];
}
