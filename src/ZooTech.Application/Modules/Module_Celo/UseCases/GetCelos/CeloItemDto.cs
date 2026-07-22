namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public sealed class CeloItemDto
{
    public long Id { get; set; }

    public string CodigoRegistro { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string CodigoVacuno { get; set; } = null!;
    public string NombreVacuno { get; set; } = null!;
    public int VecesEnCelo { get; set; }
}