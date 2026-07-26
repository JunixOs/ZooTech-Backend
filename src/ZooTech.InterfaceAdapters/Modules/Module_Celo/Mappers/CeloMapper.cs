using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

internal static class CeloMapper
{
    public static CreateCeloCommand ToCommand(CreateCeloRequest request)
    {
        return new CreateCeloCommand
        {
            VacunoId = request.VacunoId,
            EncargadoUsuarioId = request.EncargadoUsuarioId,
            FechaHora = request.FechaHora,
            Observaciones = request.Observaciones,
            CaracteristicaCodes = request.CaracteristicaCodes
        };
    }

    public static UpdateCeloCommand ToCommand(UpdateCeloRequest request , long id)
    {
        return new UpdateCeloCommand
        {
            Id = id,
            Observaciones = request.Observaciones,
            CaracteristicaCodes = request.CaracteristicaCodes
        };
    }

    public static DeleteCeloCommand ToCommand(DeleteCeloRequest request, long id)
    {
        return new DeleteCeloCommand
        {
            Id = id,
            MotivoEliminacion = request.MotivoEliminacion
        };
    }

    public static CreateCeloResponse ToResponse(CreateCeloOutput output)
    {
        return new CreateCeloResponse(
            Id: output.Id,
            Codigo: output.Codigo,
            FechaHora: output.FechaHora);
    }

    public static UpdateCeloResponse ToResponse(UpdateCeloOutput output)
    {
        return new UpdateCeloResponse(
            Id: output.Id,
            Observaciones: output.Observaciones);
    }

    public static CeloItemResponse ToResponse(CeloItemDto item)
    {
        return new CeloItemResponse
        {
            Id = item.Id,
            CodigoRegistro = item.CodigoRegistro,
            Fecha = item.Fecha,
            Hora = item.Hora,
            CodigoVacuno = item.CodigoVacuno,
            NombreVacuno = item.NombreVacuno,
            VecesEnCelo = item.VecesEnCelo,
        };
    }

    public static CeloReporteItemResponse ToResponse(CeloReporteItemDto item)
    {
        return new CeloReporteItemResponse
        {
            CodigoRegistro = item.CodigoRegistro,
            Fecha = item.Fecha,
            Hora = item.Hora,
            CodigoVacuno = item.CodigoVacuno,
            NombreVacuno = item.NombreVacuno,
            VecesEnCelo = item.VecesEnCelo,
            Caracteristicas = item.Caracteristicas,
            ListaCaracteristicas = item.ListaCaracteristicas,
            Observaciones = item.Observaciones,
            Crias = item.Crias
        };
    }

    public static ComparacionCelosResponse ToResponse(
    ComparacionCelosItemDto item)
    {
        return new ComparacionCelosResponse
        {
            Fecha = item.Fecha,
            RegistrosReales = item.RegistrosReales,
            RegistrosEstandar = item.RegistrosEstandar
        };
    }

    public static ComparacionCelosPorVacunoResponse ToResponse(
        ComparacionCelosPorVacunoItemDto item)
    {
        return new ComparacionCelosPorVacunoResponse
        {
            Periodo = item.Periodo,
            RegistrosReales = item.RegistrosReales,
            RegistrosEstandar = item.RegistrosEstandar
        };
    }

    public static ListCelosQuery ToQuery(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        Dictionary<string, string>? columnFilters
    )
    {
        return new ListCelosQuery
        {
            Search = search, 
            Page = page, 
            PageSize = pageSize, 
            FechaInicio = fechaInicio, 
            FechaFin = fechaFin, 
            ColumnFilters = columnFilters, 
        };
    }
    public static ListCelosResponse ToResponse(ListCelosOutput output)
    {
        var data = output.Result.Data.Select(ToResponse).ToList();
        var totalPages = output.Result.PageSize == 0
            ? 0
            : (int)Math.Ceiling(output.Result.TotalCount / (double)output.Result.PageSize);

        return new ListCelosResponse(
            data,
            new PaginationResponse(output.Result.Page, output.Result.PageSize, output.Result.TotalCount, totalPages));
    }

    public static ListReporteCeloGeneralResponse ToResponse(ListReporteCeloGeneralOutput output)
    {
        var data = output.Result.Data.Select(ToResponse).ToList();
        var totalPages = output.Result.PageSize == 0
            ? 0
            : (int)Math.Ceiling(output.Result.TotalCount / (double)output.Result.PageSize);

        return new ListReporteCeloGeneralResponse(
            data,
            new PaginationResponse(output.Result.Page, output.Result.PageSize, output.Result.TotalCount, totalPages));
    }

    public static VacaEnCeloResponse ToResponse(VacaEnCeloDto item)
    {
        return new VacaEnCeloResponse
        {
            Id = item.VacunoId,
            Codigo = item.Codigo,
            Nombre = item.Nombre,
            DiasRestante = item.DiasRestante,
            Estado = item.Estado,
            VecesEnCelo = item.VecesEnCelo,
            Crias = item.Crias
        };
    }
}