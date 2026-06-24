namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public sealed record UpdateTriajeCommand(
    string TipoPesoCode,
    decimal PesoKg,
    string? Observaciones,
    long? EncargadoUsuarioId);
