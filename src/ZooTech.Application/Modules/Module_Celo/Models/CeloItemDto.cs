namespace ZooTech.Application.Modules.Module_Celo.Models;

public sealed class CeloItemDto
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
