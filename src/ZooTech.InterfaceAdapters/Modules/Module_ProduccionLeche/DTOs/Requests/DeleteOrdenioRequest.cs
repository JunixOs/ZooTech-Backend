namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;

public sealed record DeleteOrdenioRequest(
    string MotivoEliminacion,
    long? ActorUsuarioId);
