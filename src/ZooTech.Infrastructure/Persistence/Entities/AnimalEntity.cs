namespace ZooTech.Infrastructure.Persistence.Entities;

public class AnimalEntity
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } = default!;
    public string RazaCode { get; set; } = default!;
    public string RazaNombre { get; set; } = default!;
    public string ProcedenciaGranja { get; set; } = default!;
    public string ProcedenciaDistrito { get; set; } = default!;
    public string ProcedenciaProvincia { get; set; } = default!;
    public string ProcedenciaDepartamento { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
