using System.Text.Json;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Rules;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;

public static class FecundacionMapper
{
    // ===== TUYO — sin cambios =====
    public static CreateFecundacionCommand ToCommand(CreateFecundacionRequest request, long? actorUsuarioId = null)
    {
        var isExternal = request.MachoExterno;
        var donorId = !isExternal ? ReadDonorId(request.MachoODonante) : null;
        var donorName = isExternal ? ReadDonorName(request.MachoODonante) : null;

        return new CreateFecundacionCommand(
            ToInternalTipo(request.TipoFecundacion) ?? string.Empty,
            request.VacunoReceptorId,
            null,
            request.FechaProcedimiento.ToDateTime(TimeOnly.MinValue),
            request.Responsable,
            ToInternalResultado(request.Resultado) ?? string.Empty,
            request.Observaciones,
            request.MachoExterno,
            donorName,
            donorId,
            actorUsuarioId,
            request.CodigoSemen,
            request.CodigoEmbrion);
    }

    public static CreateFecundacionResponse ToResponse(CreateFecundacionOutput output)
        => new(
            Id: output.Id,
            Codigo: output.Codigo,
            FechaProcedimiento: output.FechaProcedimiento);

    public static FecundacionItemResponse ToListItemResponse(FecundacionListItem item)
        => new(
            Id: item.Id,
            CodigoFecundacion: item.Codigo,
            FechaProcedimiento: item.FechaProcedimiento,
            NombreVacunoReceptor: item.VacunoReceptor,
            Responsable: item.Responsable,
            TipoFecundacion: item.Tipo,
            CodigoResultado: item.Resultado,
            NombreDonante: item.NombreDonante ?? "Sin Donante Registrado");

    // ===== DE ÉL — aditivo =====
    public static UpdateFecundacionCommand ToCommand(
        UpdateFecundacionRequest request,
        GetFecundacionForEditOutput current,
        long id)
    {
        var isExternal = request.MachoExterno ?? IsExternal(current.TipoDonante);
        var donor = ResolveDonor(request, current, isExternal);

        return new UpdateFecundacionCommand(
            id,
            ToInternalTipo(request.TipoFecundacion) ?? current.TipoFecundacionCode,
            request.VacunoReceptorId ?? current.VacunoReceptorId,
            isExternal ? FecundacionRules.TipoDonanteExterno : FecundacionRules.TipoDonanteInterno,
            donor.VacunoDonanteId,
            donor.ExternoDonanteNombre,
            request.FechaProcedimiento ?? current.FechaProcedimiento,
            request.Responsable?.Trim() ?? current.ResponsableNombre,
            ToInternalResultado(request.Resultado) ?? current.ResultadoCode,
            ToInternalEstado(request.EstadoFecundacion) ?? DefaultEstado(current.EstadoFecundacionCode),
            request.Observaciones ?? current.ObservacionesVeterinarias,
            request.CodigoSemen ?? current.CodigoSemen,
            request.CodigoEmbrion ?? current.CodigoEmbrion);
    }

    public static FecundacionEditResponse ToResponse(GetFecundacionForEditOutput output)
        => new(
            output.Id,
            output.Codigo,
            ToContractTipo(output.TipoFecundacionCode),
            new VacunoResumenResponse(output.VacunoReceptorId, output.VacunoReceptorCodigo, output.VacunoReceptorNombre),
            ToMachoODonante(output.TipoDonante, output.VacunoDonanteId, output.VacunoDonanteCodigo, output.VacunoDonanteNombre, output.ExternoDonanteNombre),
            IsExternal(output.TipoDonante),
            output.FechaProcedimiento,
            output.ResponsableNombre,
            ToContractResultado(output.ResultadoCode),
            output.CodigoSemen,
            output.CodigoEmbrion,
            output.ObservacionesVeterinarias,
            ToContractEstado(output.EstadoFecundacionCode),
            output.CreadoEn,
            output.ActualizadoEn);

    public static FecundacionOptionsResponse ToResponse(GetFecundacionOptionsOutput output)
        => new(
            output.Tipos.Select(option => ToResponse(option, ToContractTipo)).ToList(),
            output.Resultados.Select(option => ToResponse(option, ToContractResultado)).ToList(),
            output.Estados.Select(option => ToResponse(option, ToContractEstado)).ToList());

    public static FecundacionVacunoOptionResponse ToResponse(SearchFecundacionVacunoOutput output)
        => new(output.Id, output.Codigo, output.Nombre, output.Sexo);

    public static FecundacionUpdateResponse ToResponse(UpdateFecundacionOutput output)
        => new(
            output.Id,
            ToContractTipo(output.TipoFecundacionCode),
            new VacunoResumenResponse(output.VacunoReceptorId, output.VacunoReceptorCodigo, output.VacunoReceptorNombre),
            ToMachoODonante(output.TipoDonante, output.VacunoDonanteId, output.VacunoDonanteCodigo, output.VacunoDonanteNombre, output.ExternoDonanteNombre),
            output.FechaProcedimiento,
            output.ResponsableNombre,
            ToContractResultado(output.ResultadoCode),
            output.CodigoSemen,
            output.CodigoEmbrion,
            output.ObservacionesVeterinarias,
            output.ActualizadoEn,
            output.Warning);

    public static string ToContractTipo(string code)
        => Normalize(code) switch
        {
            "INSEMINACION_ARTIFICIAL" or "IA" => "inseminacion_artificial",
            "MONTA_NATURAL" or "MN" => "monta_natural",
            "TRANSFERENCIA_EMBRIONES" or "TE" => "transferencia_embriones",
            _ => code.Trim().ToLowerInvariant()
        };

    public static string? ToInternalTipo(string? code)
        => string.IsNullOrWhiteSpace(code)
            ? null
            : Normalize(code) switch
            {
                "INSEMINACION_ARTIFICIAL" or "IA" => "INSEMINACION_ARTIFICIAL",
                "MONTA_NATURAL" or "MN" => "MONTA_NATURAL",
                "TRANSFERENCIA_EMBRIONES" or "TE" => "TRANSFERENCIA_EMBRIONES",
                _ => code.Trim().ToUpperInvariant()
            };

    public static string ToContractResultado(string code)
        => Normalize(code) switch
        {
            "EXITOSO" or "EXITOSA" => "exitosa",
            "FALLIDO" or "FALLIDA" => "fallida",
            "PENDIENTE" or "PENDIENTE_CONFIRMACION" => "pendiente_confirmacion",
            _ => code.Trim().ToLowerInvariant()
        };

    public static string? ToInternalResultado(string? code)
        => string.IsNullOrWhiteSpace(code)
            ? null
            : Normalize(code) switch
            {
                "EXITOSA" or "EXITOSO" => "EXITOSO",
                "FALLIDA" or "FALLIDO" => "FALLIDO",
                "PENDIENTE_CONFIRMACION" or "PENDIENTE" => "PENDIENTE",
                _ => code.Trim().ToUpperInvariant()
            };

    public static string ToContractEstado(string code)
        => Normalize(code) switch
        {
            "EN_PROCESO" or "PENDIENTE" => "en_proceso",
            "CONFIRMADA" or "FECUNDADO" => "fecundado",
            "EN_GESTACION" or "GESTACION" => "en_gestacion",
            "FALLIDA" or "NO_FECUNDADO" => "no_fecundado",
            _ => code.Trim().ToLowerInvariant()
        };

    public static string? ToInternalEstado(string? code)
        => string.IsNullOrWhiteSpace(code)
            ? null
            : Normalize(code) switch
            {
                "EN_PROCESO" => "EN_PROCESO",
                "FECUNDADO" => "CONFIRMADA",
                "EN_GESTACION" => "EN_GESTACION",
                "NO_FECUNDADO" => "FALLIDA",
                _ => code.Trim().ToUpperInvariant()
            };

    private static FecundacionOptionResponse ToResponse(FecundacionOptionOutput output, Func<string, string> codeMapper)
        => new(codeMapper(output.Code), output.Nombre, output.Descripcion);

    private static object ToMachoODonante(string tipoDonante, long? vacunoDonanteId, string? vacunoDonanteCodigo, string? vacunoDonanteNombre, string? externoDonanteNombre)
    {
        if (IsExternal(tipoDonante))
            return externoDonanteNombre ?? string.Empty;

        return new VacunoResumenResponse(vacunoDonanteId ?? 0, vacunoDonanteCodigo ?? string.Empty, vacunoDonanteNombre ?? string.Empty);
    }

    private static DonorValues ResolveDonor(UpdateFecundacionRequest request, GetFecundacionForEditOutput current, bool isExternal)
    {
        if (request.MachoODonante is null)
        {
            return isExternal
                ? new DonorValues(null, current.ExternoDonanteNombre)
                : new DonorValues(current.VacunoDonanteId, null);
        }

        return isExternal
            ? new DonorValues(null, ReadDonorName(request.MachoODonante))
            : new DonorValues(ReadDonorId(request.MachoODonante), null);
    }

    private static long? ReadDonorId(JsonElement? value)
    {
        if (value is not { } val) return null;
        if (val.ValueKind == JsonValueKind.Number && val.TryGetInt64(out var id)) return id;
        if (val.ValueKind == JsonValueKind.String && long.TryParse(val.GetString(), out var idParsed)) return idParsed;
        throw new ArgumentException("machoODonante debe ser numérico cuando machoExterno es false.");
    }

    private static string? ReadDonorName(JsonElement? value)
    {
        if (value is not { } val) return null;
        if (val.ValueKind == JsonValueKind.String) return val.GetString();
        if (val.ValueKind == JsonValueKind.Number) return val.GetInt64().ToString();
        throw new ArgumentException("machoODonante debe ser texto cuando machoExterno es true.");
    }

    private static long? ReadDonorId(JsonElement value) => ReadDonorId((JsonElement?)value);
    private static string? ReadDonorName(JsonElement value) => ReadDonorName((JsonElement?)value);

    private static bool IsExternal(string tipoDonante)
        => string.Equals(tipoDonante, FecundacionRules.TipoDonanteExterno, StringComparison.OrdinalIgnoreCase);

    private static string DefaultEstado(string current)
        => string.IsNullOrWhiteSpace(current) ? "PENDIENTE" : current;

    private static string Normalize(string value)
        => value.Trim().Replace("-", "_").ToUpperInvariant();

    private sealed record DonorValues(long? VacunoDonanteId, string? ExternoDonanteNombre);
}
