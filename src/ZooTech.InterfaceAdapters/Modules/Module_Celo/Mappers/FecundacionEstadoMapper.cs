using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

public static class FecundacionEstadoMapper
{
    public static FecundacionEstadoResponse ToResponse(GetFecundacionEstadoOutput output)
    {
        return new FecundacionEstadoResponse(
            output.VacunoId,
            output.FecundacionId,
            output.CodigoVacuno,
            output.NombreVacuno,
            output.EstadoActual,
            output.DisponibleNuevaFecundacion,
            output.UltimaActualizacion,
            output.CodigoFecundacion,
            output.TipoFecundacion,
            output.ToroDonante,
            output.Responsable,
            output.FechaProcedimiento,
            output.Resultado,
            output.Observaciones);
    }

    public static FecundacionEstadoResponse ToResponse(UpdateFecundacionEstadoOutput output)
    {
        return new FecundacionEstadoResponse(
            output.VacunoId,
            output.FecundacionId,
            output.CodigoVacuno,
            output.NombreVacuno,
            output.EstadoActual,
            output.DisponibleNuevaFecundacion,
            output.UltimaActualizacion,
            output.CodigoFecundacion,
            output.TipoFecundacion,
            output.ToroDonante,
            output.Responsable,
            output.FechaProcedimiento,
            output.Resultado,
            output.Observaciones);
    }
}
