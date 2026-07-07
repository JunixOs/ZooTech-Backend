using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using AppRegistroVacunoReporteResponse = ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.RegistroVacunoReporteResponse;
using ObtenerRegistroVacunoReporteQuery = ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte.ObtenerRegistroVacunoReporteQuery;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;

public static class RegistroVacunoReporteMapper
{
    public static ObtenerRegistroVacunoReporteQuery ToApplicationQuery(
        long vacunoId,
        RegistroVacunoReporteRequest request)
    {
        return new ObtenerRegistroVacunoReporteQuery(vacunoId, request.Formato);
    }

    public static ListarVacunosReporteQuery ToApplicationQuery(ListadoVacunosRequest request)
    {
        return new ListarVacunosReporteQuery(
            request.FechaDesde,
            request.FechaHasta,
            request.Q,
            request.Search,
            request.Codigo,
            request.FechaRegistro,
            request.Nombre,
            request.Raza,
            request.Procedencia,
            request.Estado,
            request.EstadoRegistro,
            request.AptoPara,
            request.Formato,
            request.Page,
            request.Limit,
            request.PageSize);
    }

    public static ListadoVacunosReporteResponse ToResponse(ListarVacunosReporteResponse response)
    {
        return new ListadoVacunosReporteResponse(
            response.Data
                .Select(item => new VacunoListadoItemResponse(
                    item.Id,
                    item.Codigo,
                    item.FechaRegistro,
                    item.Nombre,
                    item.Raza,
                    item.Procedencia,
                    item.Estado,
                    item.EstadoRegistro))
                .ToList(),
            response.TotalCount,
            response.Page,
            response.PageSize,
            new ReporteVacunoResumenResponse(response.Resumen.TotalVacunos),
            new ReporteVacunoFiltrosResponse(
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

    public static RegistroVacunoReporteResponse ToResponse(AppRegistroVacunoReporteResponse response)
    {
        var v = response.Vacuno;

        return new RegistroVacunoReporteResponse(
            new RegistroVacunoDetalleResponse(
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

