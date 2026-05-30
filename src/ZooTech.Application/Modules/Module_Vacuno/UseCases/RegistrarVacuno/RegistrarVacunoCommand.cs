using MediatR;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;

/// <summary>
/// Comando CQRS para registrar un nuevo vacuno.
/// Contiene solo datos primitivos: no depende de HttpContext ni IFormFile de ASP.NET.
/// </summary>
public sealed record RegistrarVacunoCommand : IRequest<RegistrarVacunoResult>
{
    // ── Identificación ────────────────────────────────────────────────
    public required string Codigo { get; init; }
    public required string Nombre { get; init; }
    public required DateOnly FechaNacimiento { get; init; }

    // ── Adquisición ───────────────────────────────────────────────────
    /// <summary>1 = Monta, 2 = Compra (cat_tipo_adquisicion)</summary>
    public required int IdTipoAdquisicion { get; init; }
    public decimal? PrecioCompra { get; init; }

    // ── Características ───────────────────────────────────────────────
    /// <summary>FK a cat_raza</summary>
    public required int IdRaza { get; init; }
    public required string Raza { get; init; }
    /// <summary>FK a cat_color</summary>
    public required int IdColor { get; init; }
    public required string Color { get; init; }
    /// <summary>FK a cat_sexo</summary>
    public required int IdSexo { get; init; }
    public required string Sexo { get; init; }

    // ── Trazabilidad ──────────────────────────────────────────────────
    public required string CodigoPadre { get; init; }
    public required string CodigoMadre { get; init; }

    // ── Procedencia ───────────────────────────────────────────────────
    public required string NombreGranja { get; init; }
    public required int IdDistrito { get; init; }
    public required string Distrito { get; init; }
    public required int IdDepartamento { get; init; }
    public required string Departamento { get; init; }
    public required int IdProvincia { get; init; }
    public required string Provincia { get; init; }

    // ── Utilización ───────────────────────────────────────────────────
    /// <summary>FK a cat_tipo_utilizacion</summary>
    public required int IdTipoUtilizacion { get; init; }
    public required string AptoPara { get; init; }
    public required DateOnly FechaEspecificacion { get; init; }

    // ── Opcionales ────────────────────────────────────────────────────
    public string? Observaciones { get; init; }

    /// <summary>
    /// El archivo de foto se recibe como Stream para desacoplar el comando
    /// de la infraestructura HTTP (IFormFile pertenece a ASP.NET).
    /// </summary>
    public Stream? FotoStream { get; init; }
    public string? FotoNombreOriginal { get; init; }
}

/// <summary>
/// Resultado del comando una vez ejecutado exitosamente.
/// </summary>
public sealed record RegistrarVacunoResult(
    int Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicion,
    decimal? PrecioCompra,
    string? FotoUrl,
    DateTime CreadoEn
);
