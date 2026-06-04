using Microsoft.EntityFrameworkCore;

namespace ZooTech.Infrastructure.Persistence.ReadModels;

[Keyless]
public sealed class ReporteVacunoListadoRow
{
    public long Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public DateOnly FechaRegistro { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Raza { get; set; }

    public string? Procedencia { get; set; }

    public string? EstadoActualCode { get; set; }

    public string? EstadoActualNombre { get; set; }

    public int TotalRegistros { get; set; }
}
