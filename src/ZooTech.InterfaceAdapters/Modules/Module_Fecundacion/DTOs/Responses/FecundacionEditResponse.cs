using System.Text.Json.Serialization;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionEditResponse(
    long Id,
    string Codigo,
    string TipoFecundacion,
    VacunoResumenResponse VacunoReceptor,
    object MachoODonante,
    bool MachoExterno,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones,
    string EstadoFecundacion,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record VacunoResumenResponse(long Id, string Codigo, string Nombre);
