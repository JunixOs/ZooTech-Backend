using System;
using System.Linq;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Module_Vacuno.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.Domain.Module_Vacuno.ReadModels;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

internal static class VacunoMapper
{
    internal static VacunoItemResponse ToResponse(VacunoItemDto item)
        => new(
            Id: item.Id,
            Codigo: item.Codigo,
            FechaRegistro: item.FechaRegistro,
            Nombre: item.Nombre,
            Raza: item.RazaCode,
            Procedencia: item.Procedencia,
            Estado: item.IsDeleted ? "MUERTO" : "SANO");

    internal static VacunoItemResponse ToResponse(VacunoListItem item)
        => new(
            Id: item.Id,
            Codigo: item.Codigo,
            FechaRegistro: item.FechaRegistro,
            Nombre: item.Nombre,
            Raza: item.RazaCode,
            Procedencia: item.Procedencia,
            Estado: item.IsDeleted ? "MUERTO" : "SANO");

    internal static VacunoReferenceResponse ToResponse(VacunoReferenceItem item)
        => new(item.Id, item.Codigo, item.Nombre, item.SexoCode);

    internal static CreateVacunoCommand ToCommand(CreateVacunoRequest request, long? padreId, long? madreId, long granjaId)
        => new(
            request.Codigo,
            request.Nombre,
            request.FechaNacimiento,
            NormalizeCatalogCode(request.TipoAdquisicionCode),
            NormalizeCatalogCode(request.RazaCode),
            NormalizeCatalogCode(request.ColorCode),
            NormalizeCatalogCode(request.SexoCode),
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            NormalizeCatalogCode(request.AptoPara));

    internal static UpdateVacunoCommand ToCommand(UpdateVacunoRequest request, long? padreId, long? madreId, long granjaId)
        => new(
            request.Nombre,
            request.FechaNacimiento,
            NormalizeCatalogCode(request.TipoAdquisicionCode),
            NormalizeCatalogCode(request.RazaCode),
            NormalizeCatalogCode(request.ColorCode),
            NormalizeCatalogCode(request.SexoCode),
            padreId,
            madreId,
            granjaId,
            request.Observaciones,
            request.PrecioCompra,
            NormalizeCatalogCode(request.AptoPara));

    internal static DeleteVacunoCommand ToCommand(DeleteVacunoRequest request)
        => new(request.MotivoEliminacion);

    internal static VacunoResponse ToResponse(VacunoDetalleDto detail)
        => new(
            Id: detail.Id,
            Codigo: detail.Codigo,
            Nombre: detail.Nombre,
            FechaNacimiento: detail.FechaNacimiento,
            AdquisicionPor: detail.AdquisicionPor == "compra" ? "compra" : "monta",
            PrecioCompra: detail.PrecioCompra,
            Raza: detail.Raza,
            Color: detail.Color,
            Sexo: detail.Sexo == "hembra" ? "hembra" : "macho",
            CodigoPadre: detail.CodigoPadre,
            CodigoMadre: detail.CodigoMadre,
            Granja: detail.Granja,
            Distrito: detail.Distrito,
            Departamento: detail.Departamento,
            Provincia: detail.Provincia,
            CodigoDistrito: detail.CodigoDistrito,
            AptoPara: detail.AptoPara,
            FechaEspecificacion: detail.FechaEspecificacion,
            Observaciones: detail.Observaciones,
            FotoUrl: detail.FotoUrl,
            Estado: detail.Estado,
            CreadoEn: detail.CreadoEn,
            ActualizadoEn: detail.ActualizadoEn);

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

    internal static string NormalizeCatalogCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().Normalize(System.Text.NormalizationForm.FormD);
        var chars = normalized
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray();

        return new string(chars)
            .Replace(' ', '_')
            .Replace('-', '_')
            .ToUpperInvariant();
    }
}
