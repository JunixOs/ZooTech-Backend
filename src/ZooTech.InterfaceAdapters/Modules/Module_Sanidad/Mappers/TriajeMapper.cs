using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;

internal static class TriajeMapper
{
    public static CreateTriajeCommand ToCreateCommand(TriajeRequest request)
        => new CreateTriajeCommand
        {
            VacunoId = request.VacunoId, 
            TipoPesoCode = request.TipoPesoCode, 
            PesoKg = request.PesoKg,
            Observaciones = request.Observaciones, 
            EstadoRegistroCode = request.EstadoRegistroCode, 
            EncargadoUsuarioId = request.EncargadoUsuarioId
        };

    public static UpdateTriajeCommand ToUpdateCommand(TriajeRequest request)
        => new UpdateTriajeCommand{
            VacunoId = request.VacunoId, 
            TipoPesoCode = request.TipoPesoCode, 
            PesoKg = request.PesoKg,
            Observaciones = request.Observaciones, 
            EstadoRegistroCode = request.EstadoRegistroCode, 
            EncargadoUsuarioId = request.EncargadoUsuarioId
        };

    public static TriajeResponse ToResponse(CreateTriajeOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               null, output.TipoPesoCode, output.PesoKg, output.Observaciones,
               output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static TriajeResponse ToResponse(GetTriajeByIdOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               output.VacunoNombre, output.TipoPesoCode, output.PesoKg, output.Observaciones,
               output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static TriajeResponse ToResponse(UpdateTriajeOutput output)
        => new(output.Id, output.Codigo, output.FechaHora, output.VacunoId,
               null, output.TipoPesoCode, output.PesoKg, output.Observaciones,
               output.EstadoRegistroCode, output.EncargadoUsuarioId, output.CreatedAt);

    public static PagedTriajeResponse ToPagedResponse(GetAllTriajesOutput output)
        => new(
            output.Items.Select(t => new TriajeResponse(
                t.Id, t.Codigo, t.FechaHora, t.VacunoId, t.VacunoNombre,
                t.TipoPesoCode, t.PesoKg, t.Observaciones,
                t.EstadoRegistroCode, t.EncargadoUsuarioId, t.CreatedAt)).ToList().AsReadOnly(),
            output.TotalRegistros, output.Pagina, output.Tamano, output.TotalPaginas);
}
