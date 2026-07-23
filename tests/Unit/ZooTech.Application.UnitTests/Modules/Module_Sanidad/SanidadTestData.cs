using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad;

internal static class SanidadTestData
{
    public static Triaje CreateTriaje(
        long id = 1,
        string codigo = "TRI001",
        long vacunoId = 1,
        string vacunoNombre = "Luna",
        string tipoPesoCode = "CONTROL",
        decimal pesoKg = 120m,
        string? observaciones = "Sin observaciones",
        string estadoRegistroCode = "ACTIVO",
        long? encargadoUsuarioId = 10,
        DateTime? fechaHora = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        DateTime? deletedAt = null,
        string? motivoEliminacion = null)
    {
        var now = createdAt ?? DateTime.UtcNow;
        return Triaje.Rehydrate(
            id: id,
            codigo: codigo,
            fechaHora: fechaHora ?? now,
            vacunoId: vacunoId,
            vacunoNombre: vacunoNombre,
            tipoPesoCode: tipoPesoCode,
            pesoKg: pesoKg,
            observaciones: observaciones,
            estadoRegistroCode: estadoRegistroCode,
            encargadoUsuarioId: encargadoUsuarioId,
            createdBy: encargadoUsuarioId,
            updatedBy: encargadoUsuarioId,
            deletedBy: null,
            createdAt: now,
            updatedAt: updatedAt ?? now,
            deletedAt: deletedAt,
            motivoEliminacion: motivoEliminacion);
    }

    public static TriajeListadoItem CreateListadoItem(
        long id = 1,
        string codigo = "TRI001",
        long vacunoId = 1,
        string vacunoNombre = "Luna",
        string tipoPesoCode = "CONTROL",
        decimal pesoKg = 120m,
        DateTime? fechaHora = null)
        => new()
        {
            Id = id,
            Codigo = codigo,
            FechaHora = fechaHora ?? DateTime.UtcNow,
            VacunoId = vacunoId,
            VacunoNombre = vacunoNombre,
            TipoPesoCode = tipoPesoCode,
            PesoKg = pesoKg,
            Observaciones = "Sin observaciones",
            EstadoRegistroCode = "ACTIVO",
            EncargadoUsuarioId = 10,
            CreatedAt = DateTime.UtcNow,
        };

    public static TriajeHistorialItem CreateHistorialItem(
        long id = 1,
        string tipoPesoCode = "CONTROL",
        decimal pesoKg = 120m,
        DateTime? fechaHora = null)
        => new()
        {
            Id = id,
            FechaHora = fechaHora ?? DateTime.UtcNow,
            TipoPesoCode = tipoPesoCode,
            PesoKg = pesoKg,
        };
}
