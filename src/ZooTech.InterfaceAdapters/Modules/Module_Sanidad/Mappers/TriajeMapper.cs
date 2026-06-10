using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;

namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;

internal static class TriajeMapper
{
    public static CreateTriajeCommand ToCreateCommand(TriajeRequest request)
        => new(request.VacunoId, request.TipoPesoCode, request.PesoKg,
               request.Observaciones, request.EstadoRegistroCode, request.EncargadoUsuarioId);

    public static UpdateTriajeCommand ToUpdateCommand(TriajeRequest request)
        => new(request.VacunoId, request.TipoPesoCode, request.PesoKg,
               request.Observaciones, request.EstadoRegistroCode, request.EncargadoUsuarioId);

    public static object ToResponse(CreateTriajeOutput output)
        => new
        {
            id = output.Id,
            codigo = output.Codigo,
            fechaHora = output.FechaHora,
            vacunoId = output.VacunoId,
            tipoPesoCode = output.TipoPesoCode,
            pesoKg = output.PesoKg,
            observaciones = output.Observaciones,
            estadoRegistroCode = output.EstadoRegistroCode,
            encargadoUsuarioId = output.EncargadoUsuarioId,
            createdAt = output.CreatedAt
        };

    public static object ToResponse(GetTriajeByIdOutput output)
        => new
        {
            id = output.Id,
            codigo = output.Codigo,
            fechaHora = output.FechaHora,
            vacunoId = output.VacunoId,
            vacunoNombre = output.VacunoNombre,
            tipoPesoCode = output.TipoPesoCode,
            pesoKg = output.PesoKg,
            observaciones = output.Observaciones,
            estadoRegistroCode = output.EstadoRegistroCode,
            encargadoUsuarioId = output.EncargadoUsuarioId,
            createdAt = output.CreatedAt
        };

    public static object ToResponse(UpdateTriajeOutput output)
        => new
        {
            id = output.Id,
            codigo = output.Codigo,
            fechaHora = output.FechaHora,
            vacunoId = output.VacunoId,
            tipoPesoCode = output.TipoPesoCode,
            pesoKg = output.PesoKg,
            observaciones = output.Observaciones,
            estadoRegistroCode = output.EstadoRegistroCode,
            encargadoUsuarioId = output.EncargadoUsuarioId,
            createdAt = output.CreatedAt
        };

    public static object ToPagedResponse(GetAllTriajesOutput output)
        => new
        {
            data = output.Items.Select(t => new
            {
                id = t.Id,
                codigo = t.Codigo,
                fechaHora = t.FechaHora,
                vacunoId = t.VacunoId,
                vacunoNombre = t.VacunoNombre,
                tipoPesoCode = t.TipoPesoCode,
                pesoKg = t.PesoKg,
                observaciones = t.Observaciones,
                estadoRegistroCode = t.EstadoRegistroCode,
                encargadoUsuarioId = t.EncargadoUsuarioId,
                createdAt = t.CreatedAt
            }),
            totalRegistros = output.TotalRegistros,
            pagina = output.Pagina,
            tamano = output.Tamano,
            totalPaginas = output.TotalPaginas
        };
}
