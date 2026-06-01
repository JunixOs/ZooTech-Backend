namespace ZooTech.Domain.Module_Vacuno.Entities;

/// <summary>
/// Foto referencial del vacuno. Es opcional.
/// </summary>
public class VacunoFoto
{
    public int Id { get; private set; }
    public int IdVacuno { get; private set; }

    /// <summary>
    /// Ruta relativa del archivo almacenado en el servidor.
    /// El controlador construye la URL pública.
    /// </summary>
    public string RutaArchivo { get; private set; } = default!;

    public Vacuno Vacuno { get; private set; } = default!;

    private VacunoFoto() { }

    public static VacunoFoto Crear(int idVacuno, string rutaArchivo)
    {
        return new VacunoFoto
        {
            IdVacuno = idVacuno,
            RutaArchivo = rutaArchivo
        };
    }

    public void ActualizarRuta(string rutaArchivo)
    {
        RutaArchivo = rutaArchivo;
    }
}
