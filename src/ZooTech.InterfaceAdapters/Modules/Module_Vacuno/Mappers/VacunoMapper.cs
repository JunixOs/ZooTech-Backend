using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Module_Vacuno.Entities.ListarVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.Domain.Module_Vacuno.Models;
using ZooTech.Domain.Module_Vacuno.Entities;
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
            request.Codigo.Trim().ToUpperInvariant(),
            request.Nombre,
            request.FechaNacimiento,
            NormalizeCatalogCode(request.TipoAdquisicionCode),
            NormalizeCatalogCode(request.RazaCode),
            NormalizeCatalogCode(request.ColorCode),
            NormalizeSexoCode(request.SexoCode),
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            NormalizeAptoPara(request.AptoPara));

    internal static UpdateVacunoCommand ToCommand(UpdateVacunoRequest request, long? padreId, long? madreId, long granjaId)
        => new(
            request.Nombre,
            request.FechaNacimiento,
            NormalizeCatalogCode(request.TipoAdquisicionCode),
            NormalizeCatalogCode(request.RazaCode),
            NormalizeCatalogCode(request.ColorCode),
            NormalizeSexoCode(request.SexoCode),
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            NormalizeAptoPara(request.AptoPara));

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
            output.SexoCode,
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
            null,
            null,
            null);

    internal static VacunoReferenceResponse ToResponse(VacunoReferenceItem item)
        => new(item.Id, item.Codigo, item.Nombre, item.SexoCode, item.EstadoCode);

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

    private static string? NormalizeAptoPara(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToUpperInvariant() switch
        {
            "PRODUCCION_LECHE" => "LECHE",
            "LECHE" => "LECHE",
            "PRODUCCION_CARNE" => "CARNE",
            "CARNE" => "CARNE",
            "REPRODUCCION" => "REPRODUCCION",
            _ => value.Trim().ToUpperInvariant()
        };
    }

    private static string NormalizeCatalogCode(string value)
    {
        var normalized = value.Trim().ToUpperInvariant().Replace(' ', '_');
        return normalized switch
        {
            "NEGRO_Y_BLANCO" => "NEGRO_BLANCO",
            _ => normalized
        };
    }

    private static string NormalizeSexoCode(string value)
        => value.Trim().ToUpperInvariant() switch
        {
            "H" => "HEMBRA",
            "F" => "HEMBRA",
            "HEMBRA" => "HEMBRA",
            "M" => "MACHO",
            "MACHO" => "MACHO",
            _ => value.Trim().ToUpperInvariant()
        };
}
