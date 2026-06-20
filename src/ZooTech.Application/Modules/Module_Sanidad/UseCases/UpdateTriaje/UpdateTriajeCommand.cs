namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed record UpdateTriajeCommand(
    long VacunoId,
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    string EstadoRegistroCode,
    long? EncargadoUsuarioId);
