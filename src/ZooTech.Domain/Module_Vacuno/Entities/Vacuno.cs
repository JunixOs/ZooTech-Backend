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
    public string Raza { get; private set; } = default!;
    public int IdColor { get; private set; }
    public string Color { get; private set; } = default!;
    public int IdSexo { get; private set; }
    public string Sexo { get; private set; } = default!;

    // --- Trazabilidad parental ---
    public string CodigoPadre { get; private set; } = default!;
    public string CodigoMadre { get; private set; } = default!;

    // --- Procedencia ---
    public int IdGranja { get; private set; }
    public string Granja { get; private set; } = default!;
    public int IdDistrito { get; private set; }
    public string Distrito { get; private set; } = default!;
    public int IdDepartamento { get; private set; }
    public string Departamento { get; private set; } = default!;
    public int IdProvincia { get; private set; }
    public string Provincia { get; private set; } = default!;

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
        string raza,
        int idColor,
        string color,
        int idSexo,
        string sexo,
        string codigoPadre,
        string codigoMadre,
        int idGranja,
        string granja,
        int idDistrito,
        string distrito,
        int idDepartamento,
        string departamento,
        int idProvincia,
        string provincia,
        int idEstadoVivo,
        DateTime ahora)
    {
        return new Vacuno
        {
            Codigo = codigo.ToUpperInvariant(),
            Nombre = nombre,
            FechaNacimiento = fechaNacimiento,
            IdRaza = idRaza,
            Raza = raza,
            IdColor = idColor,
            Color = color,
            IdSexo = idSexo,
            Sexo = sexo,
            CodigoPadre = codigoPadre.ToUpperInvariant(),
            CodigoMadre = codigoMadre.ToUpperInvariant(),
            IdGranja = idGranja,
            Granja = granja,
            IdDistrito = idDistrito,
            Distrito = distrito,
            IdDepartamento = idDepartamento,
            Departamento = departamento,
            IdProvincia = idProvincia,
            Provincia = provincia,
            IdEstado = idEstadoVivo,
            CreadoEn = ahora,
            ActualizadoEn = ahora
        };
    }
}
