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
    private static readonly Dictionary<string, int> TiposAdquisicion = new(StringComparer.OrdinalIgnoreCase)
    {
        ["monta"] = 1,
        ["compra"] = 2
    };

    private static readonly Dictionary<string, int> Razas = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Jersey"] = 1,
        ["Holstein"] = 2,
        ["Angus"] = 3,
        ["Hereford"] = 4,
        ["Simmental"] = 5,
        ["Brown Swiss"] = 6,
        ["Brahman"] = 7,
        ["Charolais"] = 8
    };

    private static readonly Dictionary<string, int> Sexos = new(StringComparer.OrdinalIgnoreCase)
    {
        ["macho"] = 1,
        ["hembra"] = 2
    };

    private static readonly Dictionary<string, int> TiposUtilizacion = new(StringComparer.OrdinalIgnoreCase)
    {
        ["produccion_leche"] = 1,
        ["carne"] = 2,
        ["reproduccion"] = 3
    };

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
            IdTipoAdquisicion = Resolver(TiposAdquisicion, request.AdquisicionPor),
            PrecioCompra = request.PrecioCompra,
            IdRaza = Resolver(Razas, request.Raza),
            Raza = request.Raza.Trim(),
            IdColor = ResolverColor(request.Color),
            Color = request.Color.Trim(),
            IdSexo = Resolver(Sexos, request.Sexo),
            Sexo = request.Sexo.Trim().ToLowerInvariant(),
            CodigoPadre = request.CodigoPadre.Trim().ToUpperInvariant(),
            CodigoMadre = request.CodigoMadre.Trim().ToUpperInvariant(),
            NombreGranja = request.Granja.Trim(),
            IdDistrito = ResolverUbigeo(request.CodigoDistrito, request.Distrito),
            Distrito = request.Distrito.Trim(),
            IdDepartamento = ResolverUbigeo(request.CodigoDepartamento, request.Departamento),
            Departamento = request.Departamento.Trim(),
            IdProvincia = ResolverUbigeo(request.CodigoProvincia, request.Provincia),
            Provincia = request.Provincia.Trim(),
            IdTipoUtilizacion = Resolver(TiposUtilizacion, request.AptoPara),
            AptoPara = request.AptoPara.Trim(),
            FechaEspecificacion = request.FechaEspecificacion,
            Observaciones = request.Observaciones?.Trim(),

            // Desacopla IFormFile → Stream para que Application no dependa de ASP.NET
            FotoStream = request.Foto?.OpenReadStream(),
            FotoNombreOriginal = request.Foto?.FileName
        };
    }

    public static string NombreRaza(int id) => Razas.FirstOrDefault(x => x.Value == id).Key ?? $"Raza {id}";
    public static string NombreSexo(int id) => Sexos.FirstOrDefault(x => x.Value == id).Key ?? "hembra";
    public static string NombreTipoAdquisicion(int id) => TiposAdquisicion.FirstOrDefault(x => x.Value == id).Key ?? "monta";
    public static string NombreTipoUtilizacion(int id) => TiposUtilizacion.FirstOrDefault(x => x.Value == id).Key ?? "produccion_leche";
    public static int IdTipoAdquisicion(string valor) => Resolver(TiposAdquisicion, valor);
    public static int IdRaza(string valor) => Resolver(Razas, valor);
    public static int IdSexo(string valor) => Resolver(Sexos, valor);
    public static int IdTipoUtilizacion(string valor) => Resolver(TiposUtilizacion, valor);
    public static int IdColor(string valor) => ResolverColor(valor);
    public static int IdUbigeo(string valor) => ResolverUbigeo(valor);

    private static int Resolver(Dictionary<string, int> catalogo, string valor)
    {
        if (catalogo.TryGetValue(valor.Trim(), out var id))
            return id;

        return 0;
    }

    private static int ResolverColor(string color)
    {
        var normalizado = color.Trim().ToLowerInvariant();
        return normalizado switch
        {
            "negro" => 1,
            "blanco" => 2,
            "marron" or "marrón" => 3,
            "gris" => 4,
            "rojizo" or "rojo" => 5,
            _ => 1
        };
    }

    public static int IdUbigeo(string? codigo, string valor) => ResolverUbigeo(codigo, valor);

    private static int ResolverUbigeo(string? codigo, string valor)
    {
        return int.TryParse(codigo, out var id) && id > 0
            ? id
            : ResolverUbigeo(valor);
    }

    private static int ResolverUbigeo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return 0;

        unchecked
        {
            var hash = 17;
            foreach (var character in valor.Trim().ToUpperInvariant())
            {
                hash = (hash * 31) + character;
            }

            return Math.Abs(hash % 10000) + 1;
        }
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
