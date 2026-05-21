namespace ZooTech.Domain.Module_Vacuno.Entities;

/// <summary>
/// Registro de la especialización o uso del vacuno (producción leche, carne, reproducción).
/// </summary>
public class VacunoUtilizacionHistorial
{
    public int Id { get; private set; }
    public int IdVacuno { get; private set; }
    public int IdTipoUtilizacion { get; private set; }
    public DateOnly FechaEspecificacion { get; private set; }
    public string? Observaciones { get; private set; }

    public Vacuno Vacuno { get; private set; } = default!;

    private VacunoUtilizacionHistorial() { }

    public static VacunoUtilizacionHistorial Crear(
        int idVacuno,
        int idTipoUtilizacion,
        DateOnly fechaEspecificacion,
        string? observaciones)
    {
        return new VacunoUtilizacionHistorial
        {
            IdVacuno = idVacuno,
            IdTipoUtilizacion = idTipoUtilizacion,
            FechaEspecificacion = fechaEspecificacion,
            Observaciones = observaciones
        };
    }
}