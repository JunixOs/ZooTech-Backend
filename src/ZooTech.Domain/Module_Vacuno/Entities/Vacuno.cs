using ZooTech.Domain.Module_Vacuno.Enums;

namespace ZooTech.Domain.Module_Vacuno.Entities;

/// <summary>
/// Aggregate root del módulo Vacuno.
/// Representa a un bovino registrado en el sistema.
/// </summary>
public class Vacuno
{
    public int Id { get; private set; }

    // --- Identificación ---
    public string Codigo { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public DateOnly FechaNacimiento { get; private set; }

    // --- Características físicas ---
    public int IdRaza { get; private set; }
    public int IdColor { get; private set; }
    public int IdSexo { get; private set; }

    // --- Trazabilidad parental ---
    public string CodigoPadre { get; private set; } = default!;
    public string CodigoMadre { get; private set; } = default!;

    // --- Procedencia ---
    public int IdGranja { get; private set; }
    public int IdDistrito { get; private set; }
    public int IdDepartamento { get; private set; }
    public int IdProvincia { get; private set; }

    // --- Estado ---
    public int IdEstado { get; private set; }

    // --- Auditoría ---
    public DateTime CreadoEn { get; private set; }
    public DateTime ActualizadoEn { get; private set; }

    // --- Relaciones navegables (cargadas por EF) ---
    public VacunoAdquisicion Adquisicion { get; private set; } = default!;
    public VacunoFoto? Foto { get; private set; }
    public VacunoUtilizacionHistorial Utilizacion { get; private set; } = default!;

    // Constructor privado: EF Core lo requiere
    private Vacuno() { }

    /// <summary>
    /// Factory method: única forma de crear un Vacuno válido desde el exterior.
    /// Encapsula las invariantes del dominio.
    /// </summary>
    public static Vacuno Crear(
        string codigo,
        string nombre,
        DateOnly fechaNacimiento,
        int idRaza,
        int idColor,
        int idSexo,
        string codigoPadre,
        string codigoMadre,
        int idGranja,
        int idDistrito,
        int idDepartamento,
        int idProvincia,
        int idEstadoVivo,
        DateTime ahora)
    {
        return new Vacuno
        {
            Codigo = codigo.ToUpperInvariant(),
            Nombre = nombre,
            FechaNacimiento = fechaNacimiento,
            IdRaza = idRaza,
            IdColor = idColor,
            IdSexo = idSexo,
            CodigoPadre = codigoPadre.ToUpperInvariant(),
            CodigoMadre = codigoMadre.ToUpperInvariant(),
            IdGranja = idGranja,
            IdDistrito = idDistrito,
            IdDepartamento = idDepartamento,
            IdProvincia = idProvincia,
            IdEstado = idEstadoVivo,
            CreadoEn = ahora,
            ActualizadoEn = ahora
        };
    }
}