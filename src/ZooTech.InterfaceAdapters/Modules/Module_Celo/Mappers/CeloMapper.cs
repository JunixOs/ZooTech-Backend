using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

internal static class CeloMapper
{
    public static CreateCeloCommand ToCommand(CreateCeloRequest request)
    {
        return new CreateCeloCommand(
            VacunoId: request.VacunoId,
            EncargadoUsuarioId: request.EncargadoUsuarioId,
            FechaHora: request.FechaHora,
            Observaciones: request.Observaciones,
            CaracteristicaCodes: request.CaracteristicaCodes);
    }

    public static UpdateCeloCommand ToCommand(UpdateCeloRequest request)
    {
        return new UpdateCeloCommand(
            Id: request.Id,
            Observaciones: request.Observaciones,
            CaracteristicaCodes: request.CaracteristicaCodes);
    }

    public static DeleteCeloCommand ToCommand(DeleteCeloRequest request, long id)
    {
        return new DeleteCeloCommand(
            Id: id,
            MotivoEliminacion: request.MotivoEliminacion);
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
}
