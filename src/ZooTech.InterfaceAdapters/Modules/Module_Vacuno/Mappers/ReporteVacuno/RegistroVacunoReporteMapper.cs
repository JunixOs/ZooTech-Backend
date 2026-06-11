using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;

public static class RegistroVacunoReporteMapper
{
    public static ObtenerRegistroVacunoReporteQuery ToApplicationQuery(
        long vacunoId,
        RegistroVacunoReporteQueryDto dto)
    {
        return new ObtenerRegistroVacunoReporteQuery(vacunoId, dto.Formato);
    }

    public static RegistroVacunoReporteResponseDto ToDto(RegistroVacunoReporteResponse response)
    {
        var v = response.Vacuno;

        return new RegistroVacunoReporteResponseDto(
            new RegistroVacunoDetalleDto(
                v.Id,
                v.Codigo,
                v.Nombre,
                v.FechaNacimiento,
                v.AdquisicionPor,
                v.PrecioCompra,
                v.Raza,
                v.Color,
                v.Sexo,
                v.CodigoPadre,
                v.CodigoMadre,
                v.CodigoAbuelo,
                v.CodigoAbuela,
                v.Granja,
                v.Distrito,
                v.Departamento,
                v.Provincia,
                v.Procedencia,
                v.AptoPara,
                v.FechaEspecificacion,
                v.Observaciones,
                v.FotoUrl,
                v.Estado,
                v.FechaRegistro,
                v.CreadoEn,
                v.ActualizadoEn,
                v.FotoId,
                v.FotoNombreOriginal,
                v.FotoNombreAlmacenado,
                v.FotoRuta,
                v.FotoExtension,
                v.FotoTamanoBytes,
                v.EstadoActualCode,
                v.EstadoActualNombre,
                v.FechaEstado,
                v.MotivoEstado,
                v.FechaAdquisicion,
                v.CreadoPor,
                v.ActualizadoPor),
            response.Historial,
            response.DownloadUrl);
    }
}

