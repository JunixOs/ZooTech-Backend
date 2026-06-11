using ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.RegistrarCelo;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

internal static class CeloMapper
{
    public static RegistrarCeloCommand ToCommand(RegistrarCeloRequest request)
    {
        return new RegistrarCeloCommand(
            VacunoId: request.VacunoId,
            EncargadoUsuarioId: request.EncargadoUsuarioId,
            FechaHora: request.FechaHora,
            Observaciones: request.Observaciones,
            CaracteristicaCodes: request.CaracteristicaCodes);
    }

    public static EditarCeloCommand ToCommand(EditarCeloRequest request)
    {
        return new EditarCeloCommand(
            Id: request.Id,
            Observaciones: request.Observaciones,
            CaracteristicaCodes: request.CaracteristicaCodes);
    }

    public static EliminarCeloCommand ToCommand(EliminarCeloRequest request, long id)
    {
        return new EliminarCeloCommand(
            Id: id,
            MotivoEliminacion: request.MotivoEliminacion);
    }

    public static RegistrarCeloResponse ToResponse(RegistrarCeloOutput output)
    {
        return new RegistrarCeloResponse(
            Id: output.Id,
            Codigo: output.Codigo,
            FechaHora: output.FechaHora);
    }

    public static EditarCeloResponse ToResponse(EditarCeloOutput output)
    {
        return new EditarCeloResponse(
            Id: output.Id,
            Observaciones: output.Observaciones);
    }

    public static CeloItemResponse ToResponse(CeloListItemDto item)
    {
        return new CeloItemResponse
        {
            CodigoRegistro = item.CodigoRegistro,
            Fecha = item.Fecha,
            Hora = item.Hora,
            CodigoVacuno = item.CodigoVacuno,
            NombreVacuno = item.NombreVacuno,
            VecesEnCelo = item.VecesEnCelo
        };
    }
}
