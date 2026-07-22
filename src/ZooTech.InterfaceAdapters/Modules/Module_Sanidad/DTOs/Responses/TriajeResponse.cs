using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

public sealed record TriajeResponse(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string? VacunoNombre,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId,
    DateTime CreatedAt);

public sealed record PagedTriajeResponse(
    IReadOnlyList<TriajeResponse> Data,
    PaginationResponse Pagination);
