namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public sealed record UpdateOrdenioCommand(
    DateTime FechaHora,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    long? ActorUsuarioId);
