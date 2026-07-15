using System.Text.Json.Serialization;
using ZooTech.InterfaceAdapters.Utils;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;

public sealed record UpdateOrdenioRequest(
    [property: JsonConverter(typeof(SqlDateTimeJsonConverter))]
    DateTime FechaHora,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones);
