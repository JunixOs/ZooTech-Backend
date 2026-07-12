using System.Text.Json.Serialization;
using ZooTech.InterfaceAdapters.Utils;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;

public sealed record CreateOrdenioRequest(
    string Codigo,
    [property: JsonConverter(typeof(SqlDateTimeJsonConverter))]
    DateTime FechaHora,
    long VacunoId,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones);
