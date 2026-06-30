using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
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

    public static ListarVacunosReporteQuery ToApplicationQuery(ListadoVacunosReporteQueryDto dto)
    {
        return new ListarVacunosReporteQuery(
            dto.FechaDesde,
            dto.FechaHasta,
            dto.Q,
            dto.Codigo,
            dto.FechaRegistro,
            dto.Nombre,
            dto.Raza,
            dto.Procedencia,
            dto.Estado,
            dto.EstadoRegistro,
            dto.AptoPara,
            dto.Formato,
            dto.Page,
            dto.Limit);
    }

    public static ListadoVacunosReporteResponseDto ToDto(ListarVacunosReporteResponse response)
    {
        return new ListadoVacunosReporteResponseDto(
            response.Data
                .Select(item => new VacunoListadoItemDto(
                    item.Id,
                    item.Codigo,
                    item.FechaRegistro,
                    item.Nombre,
                    item.Raza,
                    item.Procedencia,
                    item.Estado,
                    item.EstadoRegistro))
                .ToList(),
            new ReporteVacunoResumenDto(response.Resumen.TotalVacunos),
            new ReporteVacunoFiltrosDto(
                response.Filtros.FechaDesde,
                response.Filtros.FechaHasta,
                response.Filtros.Q,
                response.Filtros.Codigo,
                response.Filtros.FechaRegistro,
                response.Filtros.Nombre,
                response.Filtros.Raza,
                response.Filtros.Procedencia,
                response.Filtros.Estado,
                response.Filtros.EstadoRegistro,
                response.Filtros.AptoPara,
                response.Filtros.Formato),
            response.DownloadUrl);
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
                v.DiasRegistrado,
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

