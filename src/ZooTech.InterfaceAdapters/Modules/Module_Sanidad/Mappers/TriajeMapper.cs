using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;

internal static class TriajeMapper
{
    public static CreateTriajeCommand ToCreateCommand(TriajeRequest request)
        => new(request.VacunoId, request.TipoPesoCode, request.PesoKg,
               request.Observaciones, request.EncargadoUsuarioId, request.FechaHora);

    public static UpdateTriajeCommand ToUpdateCommand(UpdateTriajeRequest request)
        => new(request.TipoPesoCode, request.PesoKg,
               request.Observaciones, request.EncargadoUsuarioId);

    public static TriajeResponse ToResponse(CreateTriajeOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               null, output.TipoPesoCode, output.PesoKg, output.Observaciones,
               output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static TriajeResponse ToResponse(GetTriajeByIdOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               output.VacunoNombre, output.TipoPesoCode, output.PesoKg, output.Observaciones, output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static TriajeResponse ToResponse(UpdateTriajeOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               output.VacunoNombre, output.TipoPesoCode, output.PesoKg, output.Observaciones, output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static PagedTriajeResponse ToPagedResponse(GetAllTriajesOutput output, int page, int pageSize)
    {
        var data = output.Data.Select(FromItemOutput).ToList();
        var totalPages = (int)Math.Ceiling((double)output.TotalCount / pageSize);
        var pagination = new PaginationResponse(page, pageSize, output.TotalCount, totalPages);
        return new PagedTriajeResponse(data, pagination);
    }

    private static TriajeResponse FromItemOutput(TriajeItemOutput item)
        => new(item.Id, item.Codigo, item.FechaHora, item.VacunoId,
               item.VacunoNombre, item.TipoPesoCode, item.PesoKg, item.Observaciones,
               item.EstadoRegistroCode, item.EncargadoUsuarioId, item.CreatedAt);
}
