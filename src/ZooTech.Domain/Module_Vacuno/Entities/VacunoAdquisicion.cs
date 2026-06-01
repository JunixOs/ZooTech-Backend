namespace ZooTech.Domain.Module_Vacuno.Entities;

/// <summary>
/// Datos de adquisición del vacuno (monta o compra).
/// Entidad dependiente del aggregate Vacuno.
/// </summary>
public class VacunoAdquisicion
{
    public int Id { get; private set; }
    public int IdVacuno { get; private set; }
    public int IdTipoAdquisicion { get; private set; }

    /// <summary>
    /// Solo tiene valor cuando IdTipoAdquisicion corresponde a "Compra".
    /// </summary>
    public decimal? PrecioCompra { get; private set; }

    // Navegación
    public Vacuno Vacuno { get; private set; } = default!;

    private VacunoAdquisicion() { }

    public static VacunoAdquisicion Crear(int idVacuno, int idTipoAdquisicion, decimal? precioCompra)
    {
        return new VacunoAdquisicion
        {
            IdVacuno = idVacuno,
            IdTipoAdquisicion = idTipoAdquisicion,
            PrecioCompra = precioCompra
        };
    }

    public void Actualizar(int idTipoAdquisicion, decimal? precioCompra)
    {
        IdTipoAdquisicion = idTipoAdquisicion;
        PrecioCompra = precioCompra;
    }
}
