namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public sealed record CreateOrdenioCommand(
    string Codigo,
    DateTime FechaHora,
    long VacunoId,
    long EncargadoUsuarioId,
    decimal Litros,
    string EstadoOrdenioCode,
    string? Observaciones,
    long? ActorUsuarioId
);

