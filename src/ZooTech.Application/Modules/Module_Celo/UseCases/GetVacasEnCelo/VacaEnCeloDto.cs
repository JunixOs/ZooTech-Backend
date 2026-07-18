namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public sealed class VacaEnCeloDto
{
    public long VacunoId { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public int DiasRestante { get; set; }
    public string Estado { get; set; } = null!;
    public int VecesEnCelo { get; set; }
    public int Crias { get; set; }
}
