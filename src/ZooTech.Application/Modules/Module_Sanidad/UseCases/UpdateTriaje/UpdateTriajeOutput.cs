namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed record UpdateTriajeOutput(
    long Id,
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId,
    DateTime CreatedAt);
