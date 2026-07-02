using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;

internal static class FecundacionMapper
{
    public static CreateFecundacionCommand ToCommand(CreateFecundacionRequest request, long? actorUsuarioId = null)
    {
        return new CreateFecundacionCommand(
            TipoFecundacionCode: request.TipoFecundacionCode,
            VacunoReceptorId: request.VacunoReceptorId,
            CeloRegistroId: request.CeloRegistroId,
            FechaProcedimiento: request.FechaProcedimiento,
            ResponsableName: request.ResponsableName,
            ResultadoCode: request.ResultadoCode,
            ObservacionesVeterinarias: request.Observaciones,
            MachoExterno: request.MachoExterno,
            MachoExternoNombre: request.MachoExternoNombre,
            VacunoDonanteId: request.VacunoDonanteId,
            CreatedById: actorUsuarioId);
    }

    public static CreateFecundacionResponse ToResponse(CreateFecundacionOutput output)
    {
        return new CreateFecundacionResponse(
            Id: output.Id,
            Codigo: output.Codigo,
            FechaProcedimiento: output.FechaProcedimiento);
    }
}
