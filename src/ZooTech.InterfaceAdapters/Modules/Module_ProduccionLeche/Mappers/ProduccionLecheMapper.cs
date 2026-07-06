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
        => new CreateOrdenioCommand
        {
            Codigo = request.Codigo,
            FechaHora = request.FechaHora,
            VacunoId = request.VacunoId,
            EncargadoUsuarioId = request.EncargadoUsuarioId,
            Litros = request.Litros,
            EstadoOrdenioCode = request.EstadoOrdenioCode,
            Observaciones = request.Observaciones
        };


    public static ListOrdenioCommand ToCommand(ListOrdeniosRequest request)
        => new(
    request.Codigo,
    request.FechaHora,
    request.VacunoId,
    request.EncargadoUsuarioId,
    request.NombreEncargado,
    request.Litros,
    request.EstadoOrdenioCode,
    request.Observaciones
       );

    public static UpdateOrdenioCommand ToCommand(UpdateOrdenioRequest request)
        => new UpdateOrdenioCommand{
            FechaHora = request.FechaHora,
            EncargadoUsuarioId = request.EncargadoUsuarioId,
            Litros = request.Litros,
            EstadoOrdenioCode = request.EstadoOrdenioCode,
            Observaciones = request.Observaciones
        };

    public static DeleteOrdenioCommand ToCommand(DeleteOrdenioRequest request)
        => new DeleteOrdenioCommand
        {
            MotivoEliminacion = request.MotivoEliminacion
        };


    public static OrdenioResponse ToResponse(OrdenioOutput output)
        => ToOrdenioResponse(output);
           

    public static OrdenioResponse ToResponse(CreateOrdenioOutput output)
        => ToOrdenioResponse(output.Data);

    public static OrdenioResponse ToResponse(GetOrdenioByIdOutput output)
        => ToOrdenioResponse(output.Data);

    public static OrdenioResponse ToResponse(UpdateOrdenioOutput output)
        => ToOrdenioResponse(output.Data);

    public static ListOrdeniosResponse ToResponse(ListOrdeniosOutput output, int page, int pageSize)
    {
        var data = output.Data.Select(ToOrdenioResponse).ToList();
        var totalPages = (int)Math.Ceiling((double)output.TotalCount / pageSize);
        return new ListOrdeniosResponse(data, new PaginationResponse(page, pageSize, output.TotalCount, totalPages));
    }

    private static OrdenioResponse ToOrdenioResponse(OrdenioOutput output)
        => new(
            output.Id,
            output.Codigo,
            output.FechaHora,
            output.VacunoId,
            output.NombreVacuno,
            output.EncargadoUsuarioId,
            output.NombreCompleto,
            output.Litros,
            output.EstadoOrdenioCode,
            output.Observaciones,
            output.CreatedAt,
            output.UpdatedAt);

}
