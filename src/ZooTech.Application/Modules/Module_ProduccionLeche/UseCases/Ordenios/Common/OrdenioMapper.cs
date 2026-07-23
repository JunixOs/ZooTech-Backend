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
            ordenio.NombreVacuno,
            ordenio.EncargadoUsuarioId,
            ordenio.NombreCompleto,
            ordenio.Litros,
            ordenio.EstadoOrdenioCode,
            ordenio.Observaciones,
            ordenio.CreatedAt,
            ordenio.UpdatedAt);

    public static OrdenioListOutput ToOutputList(OrdenioList ordenio)
        => new(
            ordenio.Id,
            ordenio.Codigo,
            ordenio.FechaHora,
            ordenio.VacunoId,
            ordenio.NombreVacuno,
            ordenio.VacunoCodigo,
            ordenio.EncargadoUsuarioId,
            ordenio.NombreCompleto,
            ordenio.Litros,
            ordenio.EstadoOrdenioCode,
            ordenio.Observaciones,
            ordenio.CreatedAt,
            ordenio.UpdatedAt);


}
