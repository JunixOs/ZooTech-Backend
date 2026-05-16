using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;

public static class ProduccionLecheMapper
{
    public static CreateOrdenioCommand ToCommand(CreateOrdenioRequest request)
        => new(
            request.Codigo,
            request.FechaHora,
            request.VacunoId,
            request.EncargadoUsuarioId,
            request.Litros,
            request.EstadoOrdenioCode,
            request.Observaciones,
            request.ActorUsuarioId);

    public static UpdateOrdenioCommand ToCommand(UpdateOrdenioRequest request)
        => new(
            request.FechaHora,
            request.EncargadoUsuarioId,
            request.Litros,
            request.EstadoOrdenioCode,
            request.Observaciones,
            request.ActorUsuarioId);

    public static DeleteOrdenioCommand ToCommand(DeleteOrdenioRequest request)
        => new(request.MotivoEliminacion, request.ActorUsuarioId);

    public static CreateOrdenioResponse ToResponse(CreateOrdenioOutput output)
        => new(ToResponse(output.Data));

    public static GetOrdenioByIdResponse ToResponse(GetOrdenioByIdOutput output)
        => new(ToResponse(output.Data));

    public static UpdateOrdenioResponse ToResponse(UpdateOrdenioOutput output)
        => new(ToResponse(output.Data));

    public static ListOrdeniosResponse ToResponse(ListOrdeniosOutput output, int page, int pageSize)
    {
        var data = output.Data.Select(ToResponse).ToList();
        var total = data.Count;
        var totalPages = total == 0 ? 0 : 1;
        return new ListOrdeniosResponse(data, new PaginationResponse(page, pageSize, total, totalPages));
    }

    private static OrdenioResponse ToResponse(OrdenioOutput output)
        => new(
            output.Id,
            output.Codigo,
            output.FechaHora,
            output.VacunoId,
            output.EncargadoUsuarioId,
            output.Litros,
            output.EstadoOrdenioCode,
            output.Observaciones,
            output.CreatedAt,
            output.UpdatedAt);
}
