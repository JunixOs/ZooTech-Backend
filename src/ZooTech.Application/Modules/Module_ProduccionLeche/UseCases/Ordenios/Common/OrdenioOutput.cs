namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

public sealed record OrdenioOutput(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string NombreVacuno,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    DateTime CreatedAt,
    DateTime UpdatedAt);
