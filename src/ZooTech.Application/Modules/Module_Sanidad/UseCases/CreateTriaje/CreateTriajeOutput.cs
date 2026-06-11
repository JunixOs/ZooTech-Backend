namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public sealed record CreateTriajeOutput(
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
