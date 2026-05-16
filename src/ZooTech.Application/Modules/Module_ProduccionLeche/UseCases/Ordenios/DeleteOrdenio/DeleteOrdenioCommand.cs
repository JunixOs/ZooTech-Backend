namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public sealed record DeleteOrdenioCommand(
    string MotivoEliminacion,
    long? ActorUsuarioId);
