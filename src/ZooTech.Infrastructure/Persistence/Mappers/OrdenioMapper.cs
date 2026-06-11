using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Mappers
{
    public class OrdenioMapper
    {
        public static Ordenio ToDomain(ordenio entity)

        => Ordenio.Rehydrate(
            entity.id,
            entity.codigo,
            entity.fecha_hora,
            entity.vacuno_id,
            entity.vacuno?.nombre ?? string.Empty,
            entity.encargado_usuario_id,
            entity.litros,
            entity.estado_ordenio_code,
            entity.observaciones,
            entity.created_at,
            entity.updated_at,
            entity.deleted_at,
            entity.motivo_eliminacion,
            entity.created_by,
            entity.updated_by,
            entity.deleted_by);

        public static ordenio ToEntity(Ordenio domain) => new ordenio
        {
            codigo = domain.Codigo,
            fecha_hora = domain.FechaHora,
            vacuno_id = domain.VacunoId,
            encargado_usuario_id = domain.EncargadoUsuarioId,
            litros = domain.Litros,
            estado_ordenio_code = domain.EstadoOrdenioCode,
            observaciones = domain.Observaciones,
            created_by = domain.CreatedBy,
            updated_by = domain.UpdatedBy,
            deleted_by = domain.DeletedBy,
            created_at = domain.CreatedAt,
            updated_at = domain.UpdatedAt,
            deleted_at = domain.DeletedAt,
            motivo_eliminacion = domain.MotivoEliminacion
        };
    };
}
    

