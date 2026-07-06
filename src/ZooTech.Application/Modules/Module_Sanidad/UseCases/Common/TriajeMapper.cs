using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;

internal static class TriajeMapper
{
    public static TriajeOutput ToOutput(Triaje triaje)
        => new (
            triaje.Id,
            triaje.Codigo,
            triaje.FechaHora,
            triaje.VacunoId,
            triaje.VacunoNombre,
            triaje.TipoPesoCode,
            triaje.PesoKg,
            triaje.Observaciones,
            triaje.EstadoRegistroCode,
            triaje.EncargadoUsuarioId,
            triaje.CreatedAt);
}
