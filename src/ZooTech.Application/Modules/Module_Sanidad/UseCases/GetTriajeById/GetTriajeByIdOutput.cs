namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

public sealed record GetTriajeByIdOutput(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string VacunoNombre,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId,
    DateTime CreatedAt);
