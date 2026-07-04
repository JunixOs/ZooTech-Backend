using System;
using System.Text.Json;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;

public sealed record CreateFecundacionRequest(
    string TipoFecundacion,
    long VacunoReceptorId,
    JsonElement MachoODonante,
    bool MachoExterno,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones);

public sealed record UpdateFecundacionRequest(
    string? TipoFecundacion,
    long? VacunoReceptorId,
    JsonElement? MachoODonante,
    bool? MachoExterno,
    DateOnly? FechaProcedimiento,
    string? Responsable,
    string? Resultado,
    string? EstadoFecundacion,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones);

public sealed record FecundacionResponse(
    long Id,
    string Codigo,
    DateOnly Fecha,
    string Vacuno,
    string TipoFecundacion,
    string ToroODonante,
    string Responsable,
    string Estado);

public sealed record DeleteFecundacionRequest(string Razon);
