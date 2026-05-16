using ZooTech.Domain.Module_ProduccionLeche.Entities;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

internal static class OrdenioMapper
{
    public static OrdenioOutput ToOutput(Ordenio ordenio)
        => new(
            ordenio.Id,
            ordenio.Codigo,
            ordenio.FechaHora,
            ordenio.VacunoId,
            ordenio.EncargadoUsuarioId,
            ordenio.Litros,
            ordenio.EstadoOrdenioCode,
            ordenio.Observaciones,
            ordenio.CreatedAt,
            ordenio.UpdatedAt);
}
