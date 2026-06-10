using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

internal static class VacunoMapper
{
    public static VacunoItemResponse ToResponse(VacunoItemDto item)
    {
        return new VacunoItemResponse(
            Id: item.Id,
            Codigo: item.Codigo,
            Nombre: item.Nombre,
            RazaCode: item.RazaCode,
            SexoCode: item.SexoCode);
    }
}
