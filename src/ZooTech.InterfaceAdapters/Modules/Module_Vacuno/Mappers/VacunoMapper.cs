using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.Domain.Module_Vacuno.Models;
using ZooTech.Domain.Module_Vacuno.ReadModels;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

internal static class VacunoMapper
{
    internal static VacunoItemResponse ToResponse(VacunoListItem item)
        => new(
            Id: item.Id,
            Codigo: item.Codigo,
            Nombre: item.Nombre,
            FechaNacimiento: item.FechaNacimiento,
            RazaCode: item.RazaCode,
            Procedencia: item.Procedencia,
            Estado: item.IsDeleted ? "eliminado" : "activo",
            FechaRegistro: item.FechaRegistro);

    internal static CreateVacunoCommand ToCommand(CreateVacunoRequest request, long? padreId, long? madreId, long granjaId)
        => new(
            request.Codigo,
            request.Nombre,
            request.FechaNacimiento,
            request.TipoAdquisicionCode,
            request.RazaCode,
            request.ColorCode,
            request.SexoCode,
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            request.AptoPara);

    internal static UpdateVacunoCommand ToCommand(UpdateVacunoRequest request, long? padreId, long? madreId, long granjaId)
        => new(
            request.Nombre,
            request.FechaNacimiento,
            request.TipoAdquisicionCode,
            request.RazaCode,
            request.ColorCode,
            request.SexoCode,
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            request.AptoPara);

    internal static DeleteVacunoCommand ToCommand(DeleteVacunoRequest request)
        => new(request.MotivoEliminacion);

    internal static VacunoResponse ToResponse(CreateVacunoOutput output)
        => ToVacunoResponse(output.Data);

    internal static VacunoResponse ToResponse(GetVacunoByIdOutput output)
        => ToVacunoResponse(output.Data);

    internal static VacunoResponse ToResponse(UpdateVacunoOutput output)
        => ToVacunoResponse(output.Data);

    private static VacunoResponse ToVacunoResponse(VacunoOutput output)
        => new(
            output.Id,
            output.Codigo,
            output.Nombre,
            output.FechaNacimiento,
            output.TipoAdquisicionCode,
            output.RazaCode,
            output.ColorCode,
            output.PadreId,
            output.MadreId,
            output.GranjaId,
            output.Observaciones,
            output.FechaRegistro,
            output.CreatedAt,
            output.UpdatedAt,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

    internal static VacunoReferenceResponse ToResponse(VacunoReferenceItem item)
        => new(item.Id, item.Codigo, item.Nombre, item.SexoCode);

    internal static VacunoCatalogsResponse ToResponse(VacunoCatalogs catalogs)
        => new(
            catalogs.TiposAdquisicion.Select(ToResponse).ToList(),
            catalogs.Razas.Select(ToResponse).ToList(),
            catalogs.Colores.Select(ToResponse).ToList(),
            catalogs.Sexos.Select(ToResponse).ToList(),
            catalogs.Estados.Select(ToResponse).ToList(),
            catalogs.Utilizaciones.Select(ToResponse).ToList(),
            catalogs.Granjas.Select(item => new GranjaCatalogOptionResponse(item.Id, item.Nombre)).ToList());

    private static VacunoCatalogOptionResponse ToResponse(VacunoCatalogOption option)
        => new(option.Code, option.Nombre);
}
