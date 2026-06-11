namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public sealed record CreateTriajeCommand(
    long VacunoId,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId);
