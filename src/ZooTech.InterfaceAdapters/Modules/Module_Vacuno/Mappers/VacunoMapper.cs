using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

/// <summary>
/// Mapper manual para el módulo Vacuno.
/// Convierte: Request → Command y Result → Response.
/// Se usa mapper manual (no AutoMapper) para mantener control total y claridad.
/// </summary>
public static class VacunoMapper
{
    /// <summary>
    /// Convierte el Request HTTP en el Command de Application.
    /// Aquí se hace el puente entre IFormFile (ASP.NET) y Stream (dominio).
    /// </summary>
    public static RegistrarVacunoCommand ToCommand(RegistrarVacunoRequest request)
    {
        return new RegistrarVacunoCommand
        {
            Codigo = request.Codigo.Trim().ToUpperInvariant(),
            Nombre = request.Nombre.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            IdTipoAdquisicion = request.IdTipoAdquisicion,
            PrecioCompra = request.PrecioCompra,
            IdRaza = request.IdRaza,
            IdColor = request.IdColor,
            IdSexo = request.IdSexo,
            CodigoPadre = request.CodigoPadre.Trim().ToUpperInvariant(),
            CodigoMadre = request.CodigoMadre.Trim().ToUpperInvariant(),
            NombreGranja = request.NombreGranja.Trim(),
            IdDistrito = request.IdDistrito,
            IdDepartamento = request.IdDepartamento,
            IdProvincia = request.IdProvincia,
            IdTipoUtilizacion = request.IdTipoUtilizacion,
            FechaEspecificacion = request.FechaEspecificacion,
            Observaciones = request.Observaciones?.Trim(),

            // Desacopla IFormFile → Stream para que Application no dependa de ASP.NET
            FotoStream = request.Foto?.OpenReadStream(),
            FotoNombreOriginal = request.Foto?.FileName
        };
    }

    /// <summary>
    /// Convierte el Result del Handler en la Response HTTP 201.
    /// Construye la URL pública de la foto a partir de la ruta relativa almacenada.
    /// </summary>
    public static RegistrarVacunoResponse ToResponse(
        RegistrarVacunoResult result,
        string baseUrl)
    {
        return new RegistrarVacunoResponse
        {
            Id = result.Id,
            Codigo = result.Codigo,
            Nombre = result.Nombre,
            FechaNacimiento = result.FechaNacimiento,
            AdquisicionPor = result.TipoAdquisicion,
            PrecioCompra = result.PrecioCompra,
            FotoUrl = result.FotoUrl is not null
                ? $"{baseUrl}/files/{result.FotoUrl}"
                : null,
            CreadoEn = result.CreadoEn
        };
    }
}